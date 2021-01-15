using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    public enum ApiErrorCode
    {
        NotFound = 404,
        Success = 1,
        SuccessWithNoReturn = 1200,
        Failed = 0,
        ParameterNotFound = 1406,
        ExplicitExit = 255,
        ApiNotFound = 1404,
        ResourceNotFound = 1405,
        ResourceCircularRef = 1001
    }

    public enum ApiMethods
    {
        POST = 1,
        GET = 2,
        //PUT = 3,
        //PATCH = 4,
        //DELETE = 5,
    }

    public enum ApiRequestFormat
    {
        FormData = 1,
        Raw = 2
    }

    public enum DataReaderResult
    {
        Actual = 0,
        Formated = 1
    }

    public enum HttpHeaderType
    {
        None = 1,
        Bearer = 2
    }
}
