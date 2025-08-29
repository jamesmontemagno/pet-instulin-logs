namespace PetInsulinLogs.Models;

public enum Role
{
    Owner = 0,
    Caretaker = 1
}

public enum InjectionSite
{
    LeftShoulder = 0,
    MiddleShoulder = 1,
    RightShoulder = 2,
    Other = 3
}

public enum OnTimeFlag
{
    OnTime = 0,
    Early = 1,
    Late = 2
}
