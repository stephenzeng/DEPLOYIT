using System.Threading.Tasks;
using Antlr.Runtime.Misc;

namespace DeployIt.Common
{
    public static class Extensions
    {
        public static T? ToNullable<T>(this object input) where T : struct
        {
            if (input == null) return default(T?);

            if (input is T?) return (T?) input;
            
            int temp;
            if (int.TryParse(input.ToString(), out temp))
            {
                return temp as T?;
            }
            
            return default(T?);
        }

        public static Task WithNotifyProgress(this Task task, Action progressAction)
        {
            var finished = false;
            task.ContinueWith(c => Task.Run(() => finished = true));
            var progressTask = Task.Run(async () =>
            {
                while (!finished)
                {
                    await Task.Delay(1000);
                    progressAction();
                }
            });

            return Task.WhenAll(task, progressTask);
        }
    }
}