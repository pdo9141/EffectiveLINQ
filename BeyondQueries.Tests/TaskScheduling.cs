using System;

namespace BeyondQueries.Tests
{
    public class TaskScheduling
    {
    }

    public class ScheduledTask
    {
        public ITask Task { get; protected set; }

        public TimeSpan Internal { get; protected set; }

        public TimeSpan Expiration { get; protected set; }

        public ScheduledTask(ITask task, 
                TimeSpan runEvery, 
                TimeSpan expireIn)
        {
            Task = task;
            Internal = runEvery;
            Expiration = expireIn;
        }
    }

    public interface ITask
    {
    }

    public class AccountSynchronizationTask : ITask
    {
    }

    public class Worker
    {
        public void DoWork()
        {
            var task = new ScheduledTask(new AccountSynchronizationTask(),
                runEvery: 2.Minutes(),
                expireIn: 3.Days());
        }
    }
    
    public static class TimeSpanExtensions
    {
        public static TimeSpan Minutes(this int value)
        {
            return new TimeSpan(0, 0, value, 0);
        }

        public static TimeSpan Days(this int value)
        {
            return new TimeSpan(value, 0, 0, 0);
        }

        public static DateTime Ago(this TimeSpan value)
        {
            return DateTime.Now - value;
        }
    }
}
