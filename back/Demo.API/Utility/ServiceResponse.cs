namespace Demo.API.Utility
{
    using System.Collections.Generic;
    using System.Linq;

    using FluentValidation.Results;

    public class ServiceResponse
    {
        public ServiceResponse()
        {
            this.ErrorMessages = new List<ValidationFailure>();
        }

        public List<ValidationFailure> ErrorMessages { get; set; }

        public bool Successful => !this.ErrorMessages.Any();

        // public string Token { get; set; }
        public string Token { get; set; }
    }

    public class ServiceResponse<T> : ServiceResponse
    {
        public T Data { get; set; }
    }
}