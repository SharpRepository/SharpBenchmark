using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SharpBenchmark.AssemblyHelper
{
    public static class ActionHelper
    {
        public  static Action BuildTestAction(Type instanceType, MethodInfo method, object[] parameterValues)
        {
            // this has parameters
            var delegateMethod = CreateDelegateWithParameters(instanceType, method);

            // build the action to return
            return () => delegateMethod.DynamicInvoke(parameterValues);
        }

        private static Delegate CreateDelegateWithParameters(Type instanceType, MethodInfo method)
        {

            object instance = null;

            if (!method.IsStatic)
            {
                //var constructors = instanceType.GetConstructors();

                instance = Activator.CreateInstance(instanceType);
            }

            var parameters = method.GetParameters();
            var args = new Expression[parameters.Length];
            var parameterExpressions = new List<ParameterExpression>();
            for (var i = 0; i < args.Length; i++)
            {
                args[i] = Expression.Parameter(parameters[i].ParameterType, parameters[i].Name);
                parameterExpressions.Add((ParameterExpression)args[i]);
            }

            var callExpression = Expression.Call(instance == null ? null : Expression.Constant(instance), method, args);

            var lambdaExpression = Expression.Lambda(callExpression, parameterExpressions);

            return lambdaExpression.Compile();
        }
    }
}
