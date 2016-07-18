//
// This file is part of Biological Signals Acquisition Framework (BSA-F).
// Copyright © Adriano Repetti 2016.
//
// BSA-F is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// BSA-F is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with BSA-F.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Bsa.Hardware
{
    static class ReliabilityHelpers
    {
        public static void ExecuteAndRetryOnError(Action actionToPerform, Action<Exception> onIgnoredError, Action<Exception> onFatalError)
        {
            for (int i = 0; i < RetriesOnError; ++i)
            {
                try
                {
                    actionToPerform();

                    return;
                }
                catch (HardwareException exception)
                {
                    if (i == RetriesOnError - 1)
                    {
                        if (onFatalError != null)
                            onFatalError(exception);

                        throw;
                    }
                    else
                    {
                        if (onIgnoredError != null)
                            onIgnoredError(exception);
                    }

                    Thread.Sleep(DelayWhenRetrying);
                }
            }
        }

        private const int RetriesOnError = 3;
        private const int DelayWhenRetrying = 1000;

        private static bool IsCriticalException(Exception ex)
        {
            // This should not happen but I want to be sure we don't trigger a new exception here
            if (ex == null)
                return false;

            // An aggregate exception is critical if any of its exceptions is critical
            if (ex is AggregateException)
                return Flatten(ex).Any(x => IsCriticalException(x));

            // These errors mare not strictly critical but they're usually caused by programming errors
            // and they won't recover "automatically", moreover application may be in an inconsistent state.
            if ((ex is NullReferenceException) || (ex is ArgumentException))
                return true;

            // These exceptions simply can't be ignored and the same operation, if repeated, will probably cuase the same error
            if ((ex is StackOverflowException) || (ex is OutOfMemoryException))
                return true;

            // An access violation is always critical, no point to wait and try again
            if (ex is AccessViolationException)
                return true;

            return false;
        }

        private static IEnumerable<Exception> Flatten(Exception exception)
        {
            if (exception != null)
            {
                var aggregateException = exception as AggregateException;
                if (aggregateException != null)
                {
                    foreach (var innerException in aggregateException.InnerExceptions)
                    {
                        foreach (var deeperException in Flatten(innerException))
                            yield return deeperException;
                    }
                }
                else
                {
                    var reflectionException = exception as ReflectionTypeLoadException;
                    if (reflectionException != null)
                    {
                        foreach (var innerException in reflectionException.LoaderExceptions)
                        {
                            foreach (var deeperException in Flatten(innerException))
                                yield return deeperException;
                        }
                    }
                    else
                    {
                        yield return exception;

                        if (exception.InnerException != null)
                        {
                            foreach (var deeperException in Flatten(exception.InnerException))
                                yield return deeperException;
                        }
                    }
                }
            }
        }
    }
}
