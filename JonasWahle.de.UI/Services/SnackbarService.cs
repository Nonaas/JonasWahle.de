using JonasWahle.de.UI.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace JonasWahle.de.UI.Services
{
    public class SnackbarService(ISnackbar Snackbar) : ISnackbarService
    {
        public void ShowSuccess(string successMessage)
        {
            Snackbar.Add(
                new MarkupString($"<h4 class=\"m-0 mud-surface-text\">{successMessage}</h4>"),
                Severity.Success,
                config =>
                {
                    config.Icon = Icons.Material.Outlined.CheckCircle;
                    config.IconSize = Size.Large;
                    config.IconColor = Color.Surface;
                    config.ShowCloseIcon = false;
                    config.VisibleStateDuration = 4000;
                }
            );
        }

        public void ShowInfo(string infoMessage)
        {
            Snackbar.Add(
                new MarkupString($"<h4 class=\"m-0 mud-surface-text\">{infoMessage}</h4>"),
                Severity.Info,
                config =>
                {
                    config.Icon = Icons.Material.Outlined.Info;
                    config.IconSize = Size.Large;
                    config.IconColor = Color.Surface;
                    config.ShowCloseIcon = false;
                    config.VisibleStateDuration = 4000;
                }
            );
        }

        public void ShowWarning(string warningMessage)
        {
            Snackbar.Add(
                new MarkupString($"<h4 class=\"m-0 mud-surface-text\">{warningMessage}</h4>"),
                Severity.Warning,
                config =>
                {
                    config.Icon = Icons.Material.Outlined.Warning;
                    config.IconSize = Size.Large;
                    config.IconColor = Color.Surface;
                    config.ShowCloseIcon = false;
                    config.VisibleStateDuration = 4000;
                }
            );
        }

        public void ShowError(string errorMessage)
        {
            Snackbar.Add(
                new MarkupString($"<h4 class=\"m-0 mud-surface-text\">{errorMessage}</h4>"),
                Severity.Error,
                config =>
                {
                    config.Icon = Icons.Material.Outlined.Error;
                    config.IconSize = Size.Large;
                    config.IconColor = Color.Surface;
                    config.ShowCloseIcon = false;
                    config.VisibleStateDuration = 4000;
                }
            );
        }
    }
}
