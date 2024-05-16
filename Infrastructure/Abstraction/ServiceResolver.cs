namespace Infrastructure.Abstraction;

public delegate TService ServiceResolver<out TService>();

public delegate TService ServiceResolver<in T,out TService>(T implementation);