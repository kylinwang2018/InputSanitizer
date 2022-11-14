using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace InputSanitizer
{
    /// <summary>
    ///     The policies of InputSanitizer
    /// </summary>
    public static class PolicyCollection
    {
        /// <summary>
        ///     Add a InputSanitizer to <see cref="IServiceCollection"/> with its policies.
        /// </summary>
        /// <param name="serviceCollection">a <see cref="IServiceCollection"/></param>
        /// <param name="policies">an array of policy</param>
        /// <returns>a <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddInputSanitizer(
            this IServiceCollection serviceCollection, params InputSanitizerPolicy[] policies)
        {
            foreach (var policy in policies)
            {
                Policies[policy.Name] = policy;
            }

            return serviceCollection;
        }

        internal static Dictionary<string,InputSanitizerPolicy> Policies { get; set; } = new Dictionary<string,InputSanitizerPolicy>();
    }
}
