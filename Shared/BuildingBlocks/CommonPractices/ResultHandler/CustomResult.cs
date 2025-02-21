
namespace CommonPractices.ResultHandler
{
    public class CustomResult<DataToReturn>
    {
        public bool IsSuccess { get; }
        public DataToReturn SuccessResponse { get; }

        public string ErrorMessage { get; }
        public object ErrorResponse{ get; }

        private CustomResult(DataToReturn successResponse=default, bool success=false, string errorMessage = null, object errorResponse = null)
        {
            IsSuccess = success;
            SuccessResponse = successResponse;

            ErrorMessage = errorMessage;
            ErrorResponse = errorResponse;
        }
        
        public static CustomResult<DataToReturn> Success(DataToReturn response)
        {
            CustomResult<DataToReturn> Ok = new CustomResult<DataToReturn>(response,true);
            return Ok;
        }

        public static CustomResult<DataToReturn> Failure(string errorMessage, object errorObj=null)
        {
            CustomResult<DataToReturn> NotOK = new CustomResult<DataToReturn>(errorMessage:errorMessage,errorResponse:errorObj);
            return NotOK;
        }

    }
}
