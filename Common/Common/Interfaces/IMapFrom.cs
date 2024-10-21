using AutoMapper;

namespace Common.Interfaces;

/// <summary>
/// Used to configure multiple mapping to many classes.
/// </summary>
public interface IMapFrom
{
    void Mapping(Profile profile);
}

/// <summary>
/// used for one on one mapping.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IMapFrom<T>
{
    void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
}