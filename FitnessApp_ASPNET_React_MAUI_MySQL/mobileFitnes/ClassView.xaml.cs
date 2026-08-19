using mobileFitnes.ApiService;
using mobileFitnes.ApiService.DataObjects;
using System.Globalization;

namespace mobileFitnes;

public partial class ClassView : ContentView
{
    private ClassesInfo _classesInfo;

    public ClassView()
    {
        InitializeComponent();
        _classesInfo = MauiProgram.ServiceProvider.GetService<ClassesInfo>()!;
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        UpdateUI();
    }

    private void UpdateUI()
    {
        var data = Data;
        if (data == null) return;

        var nextDate = GetNextDate(data.StartsAt, data.Repetition);
        DateLabel.Text = nextDate.ToString("dddd, dd.MM.yyyy HH:mm") + $" - {TranslateRepetition(Data.Repetition)}";

        string maxPeople = data.MaxPeople == null ? "∞" : (data.MaxPeople.ToString() ?? "");
        SeatsLabel.Text = $"{data.SignedPeople}/{maxPeople}";

        UpdateActionButton(nextDate);
    }

    private void UpdateActionButton(DateTime nextDate)
    {
        var data = Data;
        if (data.YouSignedUp)
        {
            ActionButton.Text = "Wypisz się";
            ActionButton.IsEnabled = true;
            ActionButton.BackgroundColor = Colors.Red;
            ActionButton.TextColor = Colors.White;
        }
        else if((data.MaxPeople ?? int.MaxValue) <= data.SignedPeople)
        {
            ActionButton.Text = "Brak wolnych miejsc";
            ActionButton.IsEnabled = false;
            ActionButton.BackgroundColor = Colors.Orange;
            ActionButton.TextColor = Colors.Black;
        }
        else if(nextDate <= DateTime.Now)
        {
            ActionButton.Text = "Zakończyły się";
            ActionButton.IsEnabled = false;
            ActionButton.BackgroundColor = Colors.Orange;
            ActionButton.TextColor = Colors.Black;
        }
        else
        {
            ActionButton.Text = "Zapisz się";
            ActionButton.IsEnabled = true;
            ActionButton.BackgroundColor = Colors.Green;
            ActionButton.TextColor = Colors.Black;
        }
    }

    ClassData Data => (ClassData)BindingContext!;

    private void OnActionClicked(object sender, EventArgs e)
    {
        var data = Data;
        if (data.YouSignedUp)
        {
            _classesInfo.SignOut(data);
        }
        else
        {
            _classesInfo.SignUp(data);
        }
        UpdateUI();
    }

    private DateTime GetNextDate(DateTime start, ClassData.ClassRepetition repetition)
    {
        DateTime next = start;
        DateTime now = DateTime.Now;

        if (next > now || repetition == ClassData.ClassRepetition.None)
            return next;

        while (next < now)
        {
            next = repetition switch
            {
                ClassData.ClassRepetition.Daily => next.AddDays(1),
                ClassData.ClassRepetition.Weekly => next.AddDays(7),
                ClassData.ClassRepetition.Every2Weeks => next.AddDays(14),
                ClassData.ClassRepetition.Monthly => next.AddMonths(1),
                _ => next
            };
        }
        return next;
    }

    private string TranslateRepetition(ClassData.ClassRepetition value) => value switch
    {
        ClassData.ClassRepetition.None => "Tylko raz",
        ClassData.ClassRepetition.Daily => "Codziennie",
        ClassData.ClassRepetition.Weekly => "Co tydzień",
        ClassData.ClassRepetition.Every2Weeks => "Co dwa tygodnie",
        ClassData.ClassRepetition.Monthly => "Co miesiąc",
        _ => value.ToString()
    };
}