using Xunit;
using Tutorial3.Models;

namespace Tutorial3Tests;

public class AdvancedEmpDeptTests
{
    [Fact]
    public void ShouldReturnMaxSalary()
    {
        var emps = Database.GetEmps();

        decimal? maxSalary = emps.Max(e => e.Sal);

        Assert.Equal(5000, maxSalary);
    }

    [Fact]
    public void ShouldReturnMinSalaryInDept30()
    {
        var emps = Database.GetEmps();

        decimal? minSalary = emps.Where(e => e.DeptNo == 30).Min(e => e.Sal);

        Assert.Equal(1250, minSalary);
    }

    [Fact]
    public void ShouldReturnFirstTwoHiredEmployees()
    {
        var emps = Database.GetEmps();

        var firstTwo = emps.OrderBy(e => e.HireDate).Take(2).ToList();

        Assert.Equal(2, firstTwo.Count);
        Assert.True(firstTwo[0].HireDate <= firstTwo[1].HireDate);
    }

    [Fact]
    public void ShouldReturnAllManagers()
    {
        var emps = Database.GetEmps();

        var managers = emps.Where(e => emps.Any(m => m.Mgr == e.EmpNo)).ToList();

        Assert.All(managers, m => Assert.Contains(emps.Select(e => e.Mgr), mgr => mgr == m.EmpNo));
    }

    [Fact]
    public void ShouldReturnDeptWithEmployees()
    {
        var emps = Database.GetEmps();
        var depts = Database.GetDepts();

        var result = depts.Where(d => emps.Any(e => e.DeptNo == d.DeptNo)).ToList();

        Assert.All(result, dept => Assert.Contains(emps, e => e.DeptNo == dept.DeptNo));
    }
    [Fact]
    public void ShouldReturnAverageSalary()
    {
        var emps = Database.GetEmps();

        var avgSal = emps.Average(e => e.Sal);

        Assert.Equal(2730, avgSal); 
    }

    [Fact]
    public void ShouldReturnDeptNoAndAvgSalary()
    {
        var emps = Database.GetEmps();

        var result = emps
            .GroupBy(e => e.DeptNo)
            .Select(g => new { DeptNo = g.Key, AvgSal = g.Average(e => e.Sal) })
            .ToList();

        Assert.Equal(3, result.Count);
        Assert.Contains(result, r => r.DeptNo == 10 && r.AvgSal == 5000);
        Assert.Contains(result, r => r.DeptNo == 20 && r.AvgSal == 800);
        Assert.Contains(result, r => r.DeptNo == 30 && r.AvgSal == 1425);
    }

    [Fact]
    public void ShouldReturnEmployeeWithHighestSalary()
    {
        var emps = Database.GetEmps();

        var topEmp = emps.OrderByDescending(e => e.Sal).First();

        Assert.Equal("KING", topEmp.EName);
    }

    [Fact]
    public void ShouldReturnDeptWithHighestAvgSalary()
    {
        var emps = Database.GetEmps();

        var result = emps
            .GroupBy(e => e.DeptNo)
            .Select(g => new { DeptNo = g.Key, AvgSal = g.Average(e => e.Sal) })
            .OrderByDescending(x => x.AvgSal)
            .First();

        Assert.Equal(10, result.DeptNo);
    }

    [Fact]
    public void ShouldReturnCrossJoin()
    {
        var emps = Database.GetEmps();
        var depts = Database.GetDepts();

        var result = emps.SelectMany(e => depts, (e, d) => new { Emp = e, Dept = d }).ToList();

        Assert.Equal(emps.Count * depts.Count, result.Count);
    }
}
