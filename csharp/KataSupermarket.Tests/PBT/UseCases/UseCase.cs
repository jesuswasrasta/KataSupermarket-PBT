namespace KataSupermarket.Tests.PBT.UseCases;

public record UseCase<T1, T2>(T1 t1, T2 t2);

public record UseCase<T1, T2, T3>(T1 t1, T2 t2, T3 t3);

public record UseCase<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4);
