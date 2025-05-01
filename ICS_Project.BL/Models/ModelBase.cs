using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ICS_Project.BL.Models;

public class ModelBase : ObservableObject, IModel
{
    public Guid Id { get; set; }
}