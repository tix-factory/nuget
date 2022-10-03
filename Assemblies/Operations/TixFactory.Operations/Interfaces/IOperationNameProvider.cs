using System;

namespace TixFactory.Operations
{
    public interface IOperationNameProvider
    {
        string GetOperationName(Type operationType);
    }
}
