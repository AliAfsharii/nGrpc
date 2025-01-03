﻿using Grpc.Core;
using System.Text;

namespace nGrpc.Common
{
    public static class GrpcUtils
    {
        public static Method<T, R> CreateMethodDescriptor<T, R>(MethodType methodType, string serviceName, string methodName)
            where T : class
            where R : class
        {
            return new Method<T, R>(
                    type: methodType,
                    serviceName: serviceName,
                    name: methodName,
                    requestMarshaller: CreateMarshaller<T>(),
                    responseMarshaller: CreateMarshaller<R>());
        }

        static Marshaller<T> CreateMarshaller<T>() where T : class
        {
            return Marshallers.Create(
                serializer: GrpcSerializer<T>.ToBytes,
                deserializer: GrpcSerializer<T>.FromBytes);
        }

        static class GrpcSerializer<T> where T : class
        {
            public static byte[] ToBytes(T obj)
            {
                string st = obj.ToJson();
                return Encoding.UTF8.GetBytes(st);
            }

            public static T FromBytes(byte[] bytes)
            {
                if (bytes == null)
                    return null;

                string st = Encoding.UTF8.GetString(bytes);
                return st.ToObject<T>();
            }
        }
    }
}
