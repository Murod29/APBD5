using Xunit;
using Tutorial3.Models;

public class EmpDeptSalgradeTests
{
    [Fact]
    public void ShouldReturnAllSalesmen()
    {
        var emps = Database.GetEmps();

        List<Emp> result = emps.Where(e => e.Job == "SALESMAN").ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, e => Assert.Equal("SALESMAN", e.Job));
    }

    [Fact]
    public void ShouldReturnDept30EmpsOrderedBySalaryDesc()
    {
        var emps = Database.GetEmps();

        List<Emp> result = emps
            .Where(e => e.DeptNo == 30)
            .OrderByDescending(e => e.Sal)
            .ToList();

        Assert.Equal(2, result.Count);
        Assert.True(result[0].Sal >= result[1].Sal);
    }

    [Fact]
    public void ShouldReturnEmployeesFromChicago()
    {
        var emps = Database.GetEmps();
        var depts = Database.GetDepts();

        var chicagoDeptNos = depts
            .Where(d => d.Loc == "CHICAGO")
            .Select(d => d.DeptNo);

        List<Emp> result = emps
            .Where(e => chicagoDeptNos.Contains(e.DeptNo))
            .ToList();

        Assert.All(result, e => Assert.Equal(30, e.DeptNo));
    }

    [Fact]
    public void ShouldSelectNamesAndSalaries()
    {
        var emps = Database.GetEmps();

        var result = emps
            .Select(e => new { e.EName, e.Sal })
            .ToList();

        Assert.All(result, r =>
        {
            Assert.False(string.IsNullOrWhiteSpace(r.EName));
            Assert.True(r.Sal > 0);
        });
    }

    [Fact]
    public void ShouldJoinEmployeesWithDepartments()
    {
        var emps = Database.GetEmps();
        var depts = Database.GetDepts();

        var result = emps.Join(
                depts,
                e => e.DeptNo,
                d => d.DeptNo,
                (e, d) => new { e.EName, d.DName })
            .ToList();

        Assert.Contains(result, r => r.DName == "SALES" && r.EName == "ALLEN");
    }

    [Fact]
    public void ShouldCountEmployeesPerDepartment()
    {
        var emps = Database.GetEmps();

        var result = emps
            .GroupBy(e => e.DeptNo)
            .Select(g => new { DeptNo = g.Key, Count = g.Count() })
            .ToList();

        Assert.Contains(result, g => g.DeptNo == 30 && g.Count == 2);
    }

    [Fact]
    public void ShouldReturnEmployeesWithCommission()
    {
        var emps = Database.GetEmps();

        var result = emps
            .Where(e => e.Comm != null)
            .Select(e => new { e.EName, e.Comm })
            .ToList();

        Assert.All(result, r => Assert.NotNull(r.Comm));
    }

    [Fact]
    public void ShouldMatchEmployeeToSalaryGrade()
    {
        var emps = Database.GetEmps();
        var grades = Database.GetSalgrades();

        var result = emps
            .Where(e => grades.Any(g => e.Sal >= g.Losal && e.Sal <= g.Hisal))
            .Select(e => new
            {
                e.EName,
                Grade = grades.First(g => e.Sal >= g.Losal && e.Sal <= g.Hisal).Grade
            })
            .ToList();

        Assert.Contains(result, r => r.EName == "ALLEN" && r.Grade == 3);
    }

    [Fact]
    public void ShouldCalculateAverageSalaryPerDept()
    {
        var emps = Database.GetEmps();

        var result = emps
            .GroupBy(e => e.DeptNo)
            .Select(g => new { DeptNo = g.Key, AvgSal = g.Average(e => e.Sal) })
            .ToList();

        Assert.Contains(result, r => r.DeptNo == 30 && r.AvgSal > 1000);
    }

    [Fact]
    public void ShouldReturnEmployeesEarningMoreThanDeptAverage()
    {
        var emps = Database.GetEmps();

        var result = emps
            .Where(e =>
                e.Sal >
                emps.Where(inner => inner.DeptNo == e.DeptNo).Average(inner => inner.Sal))
            .Select(e => e.EName)
            .ToList();

        Assert.Contains("ALLEN", result);
    }
}
