Imports BusinessLayer
Imports Csla
Imports Csla.Configuration
Imports Microsoft.Extensions.DependencyInjection
Imports System

Namespace CslaFastStart
  Friend Class Program
    Private Shared Property ServiceProvider As IServiceProvider

    Public Shared Sub Main(ByVal args As String())
      Dim services = New ServiceCollection()
      services.AddCsla()
      ServiceProvider = services.BuildServiceProvider()

      Dim dpFactory = ServiceProvider.GetRequiredService(Of IDataPortalFactory)()

      Console.WriteLine("Creating a new person")
      Dim person = dpFactory.GetPortal(Of PersonEdit)().Create()
      Console.Write("Enter first name: ")
      person.FirstName = Console.ReadLine()
      Console.Write("Enter last name: ")
      person.LastName = Console.ReadLine()
      If person.IsSavable Then
        person = person.Save()
        Console.WriteLine("Added person with id {0}. First name = '{1}', last name = '{2}'.", person.Id, person.FirstName, person.LastName)
      Else
        Console.WriteLine("Invalid entry")
        For Each item In person.BrokenRulesCollection
          Console.WriteLine(item.Description)
        Next
        Console.ReadKey()
        Return
      End If

      Console.WriteLine()
      Console.WriteLine("Updating existing person")
      person = dpFactory.GetPortal(Of PersonEdit)().Fetch(person.Id)
      Console.Write("Update first name [{0}]: ", person.FirstName)
      Dim temp = Console.ReadLine()
      If Not String.IsNullOrWhiteSpace(temp) Then
        person.FirstName = temp
      End If
      Console.Write("Update last name [{0}]: ", person.LastName)
      temp = Console.ReadLine()
      If Not String.IsNullOrWhiteSpace(temp) Then
        person.LastName = temp
      End If
      If person.IsSavable Then
        person = person.Save()
        Console.WriteLine("Updated person with id {0}. First name = '{1}', last name = '{2}'.", person.Id, person.FirstName, person.LastName)
      Else
        If person.IsDirty Then
          Console.WriteLine("Invalid entry")
          For Each item In person.BrokenRulesCollection
            Console.WriteLine(item.Description)
          Next
          Console.ReadKey()
          Return
        Else
          Console.WriteLine("No changes, nothing to save")
        End If
      End If

      Console.WriteLine()
      Console.WriteLine("Deleting existing person")
      dpFactory.GetPortal(Of PersonEdit)().Delete(person.Id)
      Try
        person = dpFactory.GetPortal(Of PersonEdit)().Fetch(person.Id)
        Console.WriteLine("Person NOT deleted")
      Catch
        Console.WriteLine("Person successfully deleted")
      End Try

      Console.ReadKey()
    End Sub
  End Class
End Namespace
