using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace DeskBooker.Core.Processor
{
    public class DeskBookingRequestProcessorTests
    {
        private readonly Mock<IDeskBookingRepository> _deskBookingRepositoryMock;
        private readonly DeskBookingRequestProcessor _processor;
        private readonly DeskBookingRequest _request;
        private readonly List<Desk> _availableDesks;
        private readonly Mock<IDeskRepository> _deskRepositoryMock;

        public DeskBookingRequestProcessorTests()
        {
            _deskBookingRepositoryMock = new Mock<IDeskBookingRepository>();
            
            _request = new DeskBookingRequest
            {
                FirstName = "Thomas",
                LastName = "Huber",
                Email = "thomas.huber@emai.com",
                Date = new DateTime(2020, 1, 28)
            };

            _availableDesks = new List<Desk> { new Desk { Id = 7 } };//booking should be possible with one desk available 
            _deskRepositoryMock = new Mock<IDeskRepository>();
            _deskRepositoryMock.Setup(x => x.GetAvailableDesks(_request.Date))
                .Returns(_availableDesks);

            _processor = new DeskBookingRequestProcessor(
                _deskBookingRepositoryMock.Object, _deskRepositoryMock.Object);
            //common code moved to constructor
        }
        [Fact]
        public void ShouldReturnDeskBookingResultWithRequestValue()
        {
            //arrange
            
            
            //act
            DeskBookingResult result = _processor.BookDesk(_request);

            //assert
            Assert.NotNull(result);
            Assert.Equal(_request.FirstName, result.FirstName);
            Assert.Equal(_request.LastName, result.LastName);
            Assert.Equal(_request.Email, result.Email);
            Assert.Equal(_request.Date, result.Date);
        }

        [Fact]
        public void ShouldThrowExceptionIfRequestIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => _processor.BookDesk(null));

            Assert.Equal("request",exception.ParamName);
        }
    
        [Fact]
        public void ShouldSaveDeskBooking()
        {
            DeskBooking savedDeskBooking = null;
            _deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))//as long as Save is called with type DeskBooking use callback
                .Callback<DeskBooking>(deskbooking => {
                    savedDeskBooking = deskbooking;//saves the arguments that the Save was called with locally for testing.
                });
            _processor.BookDesk(_request);

            _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Once);//asserts if Save was called only once
            Assert.NotNull(savedDeskBooking);
            Assert.Equal(_request.FirstName, savedDeskBooking.FirstName);
            Assert.Equal(_availableDesks.First().Id, savedDeskBooking.DeskId);
        }

        [Fact]
        public void ShouldNotSaveDeskBookingIfDeskIsNotAvailable()
        {
            //Ensure no desk is available
            _availableDesks.Clear();//clears available desks, no desks available

            _processor.BookDesk(_request);//after booking the Save should not be called as no desks are available

            _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Never);//asserts if Save was never called once 

        }

        [Theory]//Data driven test uses theory att
        [InlineData(DeskBookingResultCode.Success, true)]//when desk available
        [InlineData(DeskBookingResultCode.NoDeskAvailable, false)]//when desk is not available
        public void ShouldReturnExpectedResultCode(
            DeskBookingResultCode expectedResultCode, bool isDeskAvailable)
        {
            if (!isDeskAvailable)
            {
                _availableDesks.Clear();
            }

            var result = _processor.BookDesk(_request);

            Assert.Equal(expectedResultCode, result.Code);
        }
    }
}
