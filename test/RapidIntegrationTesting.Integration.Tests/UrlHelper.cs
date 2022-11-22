using RapidIntegrationTesting.Utility.Extensions;
using Testing.Integration.TestWebApi.Controllers;

namespace RapidIntegrationTesting.Integration.Tests;

internal static class UrlHelper
{
    private static readonly string UsersControllerRoute = nameof(UsersController).GetRelativePathToController();
    private static readonly string AdminsControllerRoute = nameof(AdminsController).GetRelativePathToController();
    private static readonly string SignalRControllerRoute = nameof(SignalRController).GetRelativePathToController();

    public static readonly string GetUsersDataRoute = $"{UsersControllerRoute}";
    public static readonly string GetAdminsDataRoute = $"{AdminsControllerRoute}";
    public static readonly string PostDoSomethingAndNotifySignalR = $"{SignalRControllerRoute}";
}