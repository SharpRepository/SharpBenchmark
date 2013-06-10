using System;
using SharpBenchmark.Wpf.Models;
using SharpRepository.Repository;

namespace SharpBenchmark.Wpf
{
    public static class Repositories
    {
        public static IRepository<AssemblyFilePath, Guid> AssemblyFileRepository = RepositoryFactory.GetInstance<AssemblyFilePath, Guid>();
//        public static IRepository<AssemblyFilePath, Guid> AssemblyFileRepository = RepositoryFactory.GetInstance<AssemblyFilePath, Guid>();
    }
}
