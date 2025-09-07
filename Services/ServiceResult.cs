namespace AutoShopAPI.Services
{
    public enum ServiceError
    {
        None = 0,
        NotFound = 1,
        Conflict = 2,
        Validation = 3,
        BadRequest = 4
    }

    public sealed class ServiceResult<T>
    {
        public bool Success { get; private set; }
        public ServiceError Error { get; private set; }
        public string? Message { get; private set; }
        public T? Value { get; private set; }

        private ServiceResult() { }

        public static ServiceResult<T> Ok(T value)
        {
            return new ServiceResult<T>
            {
                Success = true,
                Error = ServiceError.None,
                Value = value
            };
        }

        public static ServiceResult<T> NotFound(string? message = null)
        {
            return new ServiceResult<T>
            {
                Success = false,
                Error = ServiceError.NotFound,
                Message = message
            };
        }

        public static ServiceResult<T> Conflict(string? message = null)
        {
            return new ServiceResult<T>
            {
                Success = false,
                Error = ServiceError.Conflict,
                Message = message
            };
        }

        public static ServiceResult<T> Validation(string? message = null)
        {
            return new ServiceResult<T>
            {
                Success = false,
                Error = ServiceError.Validation,
                Message = message
            };
        }

        public static ServiceResult<T> BadRequest(string? message = null)
        {
            return new ServiceResult<T>
            {
                Success = false,
                Error = ServiceError.BadRequest,
                Message = message
            };
        }
    }
}


