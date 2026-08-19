using mobileFitnes.ApiService;
using mobileFitnes.ApiService.DataObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace mobileFitnes;

public class ClassesInfo
{
    readonly IApiEndpoints _api;

    public ObservableCollection<ClassData> VisibleClasses { get; private set; } = new();
    List<ClassData> all = new();

    public bool OnlyMine { get; private set; }

    public event Action? Updated;

    public ClassesInfo(IApiEndpoints api)
    {
        _api = api;
    }

    public async Task Load()
    {
        var res = await _api.GetClasses();

        if (!res.IsSuccessStatusCode || res.Content == null)
            return;

        all = res.Content;
        OnlyMine = false;
        ApplyFilter();
    }

    public void SetOnlyMine(bool value)
    {
        OnlyMine = value;
        ApplyFilter();
    }

    void ApplyFilter()
    {
        IEnumerable<ClassData> src = all;

        if (OnlyMine)
            src = src.Where(x => x.YouSignedUp);

        VisibleClasses = new ObservableCollection<ClassData>(src);

        Updated?.Invoke();
    }

    public void Update(ClassData changed)
    {
        var idx = all.FindIndex(x => x.Id == changed.Id);
        if (idx >= 0)
            all[idx] = changed;

        ApplyFilter();
    }
    public void SignUp(ClassData c)
    {
        c.YouSignedUp = true;
        c.SignedPeople++;
        _api.SignUpForClass(c.Id);
        Update(c);
    }
    public void SignOut(ClassData c)
    {
        c.YouSignedUp = false;
        c.SignedPeople--;
        _api.LeaveClass(c.Id);
        Update(c);
    }
}
