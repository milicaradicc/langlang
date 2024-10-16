using System;
using System.Collections.Generic;
using LangLang.Models;

namespace LangLang.Services;

public interface IUserService
{
    public List<User> GetAll();
    public User? GetById(int id);

    public void Add(string? firstName, string? lastName, string? email, string? password, Gender gender, string? phone,
        Education? education = null, List<Language>? languages = null);

    public void Update(int id, string firstName, string lastName, string password, Gender gender, string phone,
        Education? education = null, int penaltyPoints = -1);

    public void Delete(int id);
    public User? Login(string email, string password);
    public void Logout();

}