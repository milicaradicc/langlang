using System;
using System.Collections.Generic;
using LangLang.Models;

namespace LangLang.Repositories;

public interface IPenaltyPointRepository
{
    public List<PenaltyPoint> GetAll();
    public PenaltyPoint? GetById(int id);
    public int GenerateId();
    public void Add(PenaltyPoint course);
    public void Update(PenaltyPoint course);
    public void Delete(int id);
}