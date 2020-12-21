using DeskBooker.Core.Domain;
using System;
using System.Diagnostics;
using Xunit;

namespace DeskBooker.Core.Processor
{
    public class DeskBookingRequestProcessorTests
    {
        [Fact]
        public void ShouldReturnDeskBookingResultWithRequestValue()
        {
            //arrange
            var request = new DeskBookingRequest {
                FirstName = "Thomas",
                LastName = "Huber",
                Email = "thomas.huber@emai.com",
                Date = new DateTime(2020, 1, 28)
            };
            var processor = new DeskBookingRequestProcessor();
            
            //act
            DeskBookingResult result = processor.BookDesk(request);

            //assert
            Assert.NotNull(result);
            Assert.Equal(request.FirstName, result.FirstName);
            Assert.Equal(request.LastName, result.LastName);
            Assert.Equal(request.Email, result.Email);
            Assert.Equal(request.Date, result.Date);
        }
    }
}
