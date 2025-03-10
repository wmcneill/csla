﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.TestHelpers;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace Csla.Test.BasicModern
{
  [TestClass]
  public class BasicModernTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public void EditLevelsWorkWithMobileFormatter()
    {
      var oldSetting = Configuration.ConfigurationManager.AppSettings["CslaSerializationFormatter"];
      try
      {
        Configuration.ConfigurationManager.AppSettings.Set("CslaSerializationFormatter", "MobileFormatter");

        var root = NewRoot();

        var originalRootId = root.Id;

        root.BeginEdit();
        root.Id = originalRootId + 1;
        root.CancelEdit();

        Assert.AreEqual(originalRootId, root.Id);

        root.BeginEdit();
        root.Id = originalRootId + 1;
        root.ApplyEdit();

        Assert.AreEqual(originalRootId + 1, root.Id);
      }
      finally
      {
        Configuration.ConfigurationManager.AppSettings.Set("CslaSerializationFormatter", oldSetting);
      }
    }

    [TestMethod]
    public void CloneWorkswithMobileFormatter()
    {
      var oldSetting = Configuration.ConfigurationManager.AppSettings["CslaSerializationFormatter"];
      try
      {
        Configuration.ConfigurationManager.AppSettings.Set("CslaSerializationFormatter", "MobileFormatter");

        var original = NewRoot();

        original.Name = "Test Root";

        var child = original.Children.AddNew();

        child.Name = "TestChild";

        var copy = original.Clone();

        Assert.IsFalse(ReferenceEquals(original, copy));
        Assert.AreEqual(original.Id, copy.Id);
        Assert.AreEqual(original.IsDirty, copy.IsDirty);
        Assert.AreEqual(original.IsSelfDirty, copy.IsSelfDirty);
        
        for(var i = 0; i < original.Children.Count; i++)
        {
          Assert.IsFalse(ReferenceEquals(original.Children[i], copy.Children[i]));
          Assert.AreEqual(original.Children[i].Name, copy.Children[i].Name);
          Assert.AreEqual(original.Children[i].IsDirty, copy.Children[i].IsDirty);
        }
      }
      finally
      {
        Configuration.ConfigurationManager.AppSettings.Set("CslaSerializationFormatter", oldSetting);
      }
    }

    [TestMethod]
    public void CreateGraph()
    {
      var graph = NewRoot();
      Assert.IsTrue(graph.IsNew, "IsNew");
      Assert.IsFalse(graph.IsValid, "IsValid");
      Assert.AreEqual(0, graph.Children.Count, "Children count");
    }

    [TestMethod]
    public void MakeOldMetastateEvents()
    {
      var graph = NewRoot();
      var changed = new List<string>();
      graph.PropertyChanged += (o, e) =>
      {
        changed.Add(e.PropertyName);
      };

      graph.MakeOld();

      // TODO: Are these assumptions about what should happen actually correct?
      Assert.IsTrue(changed.Contains("IsDirty"), "IsDirty did not change as expected");
      Assert.IsTrue(changed.Contains("IsSelfDirty"), "IsSelfDirtynot as expected");
      Assert.IsFalse(changed.Contains("IsValid"), "IsValid changed; that was not expected");
      Assert.IsFalse(changed.Contains("IsSelfValid"), "IsSelfValid changed; that was not expected");
      Assert.IsTrue(changed.Contains("IsSavable"), "IsSavable did not change as expected");
      Assert.IsTrue(changed.Contains("IsNew"), "IsNew did not change as expected");
      Assert.IsFalse(changed.Contains("IsDeleted"), "IsDeleted changed; that was not expected");
    }

    [TestMethod]
    public void MarkDeletedMetastateEvents()
    {
      var graph = NewRoot();
      graph.Name = "abc";
      graph = graph.Save();
      var changed = new List<string>();
      graph.PropertyChanged += (o, e) =>
      {
        changed.Add(e.PropertyName);
      };

      graph.Delete();

      Assert.IsTrue(changed.Contains("IsDirty"), "IsDirty did not change as was expected");
      Assert.IsTrue(changed.Contains("IsSelfDirty"), "IsSelfDirty did not change as we expected");
      Assert.IsFalse(changed.Contains("IsValid"), "IsValid changed; that was not expected");
      Assert.IsFalse(changed.Contains("IsSelfValid"), "IsSelfValid changed; that was not expected");
      Assert.IsTrue(changed.Contains("IsSavable"), "IsSavable did not change as we expected");
      Assert.IsFalse(changed.Contains("IsNew"), "IsNew changed; that was not expected");
      Assert.IsTrue(changed.Contains("IsDeleted"), "IsDeleted did not change as we expected");
    }

    [TestMethod]
    public void RootChangedMetastateEventsId()
    {
      Csla.ApplicationContext.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Xaml;
      var graph = NewRoot();
      var changed = new List<string>();
      graph.PropertyChanged += (o, e) =>
        {
          changed.Add(e.PropertyName);
        };

      graph.Id = 123;

      Assert.IsTrue(changed.Contains("Id"), "Id did not change as we expected");
      Assert.IsFalse(changed.Contains("IsDirty"), "IsDirty changed; that was not expected");
      Assert.IsFalse(changed.Contains("IsSelfDirty"), "IsSelfDirty changed; that was not expected");
      Assert.IsTrue(changed.Contains("IsValid"), "IsValid did not change as we expected");
      Assert.IsTrue(changed.Contains("IsSelfValid"), "IsSelfValid did not change as we expected");
      Assert.IsTrue(changed.Contains("IsSavable"), "IsSavable did not change as we expected");
      Assert.IsFalse(changed.Contains("IsNew"), "IsNew changed; that was not expected");
      Assert.IsFalse(changed.Contains("IsDeleted"), "IsDeleted changed; that was not expected");
    }

    [TestMethod]
    public void RootChangedMetastateEventsName()
    {
      Csla.ApplicationContext.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Xaml;
      var graph = NewRoot();
      var changed = new List<string>();
      graph.PropertyChanged += (o, e) =>
      {
        changed.Add(e.PropertyName);
      };

      graph.Name = "abc";

      Assert.IsTrue(changed.Contains("Name"), "Name");
      Assert.IsFalse(changed.Contains("IsDirty"), "IsDirty");
      Assert.IsFalse(changed.Contains("IsSelfDirty"), "IsSelfDirty");
      Assert.IsTrue(changed.Contains("IsValid"), "IsValid");
      Assert.IsTrue(changed.Contains("IsSelfValid"), "IsSelfValid");
      Assert.IsTrue(changed.Contains("IsSavable"), "IsSavable");
      Assert.IsFalse(changed.Contains("IsNew"), "IsNew");
      Assert.IsFalse(changed.Contains("IsDeleted"), "IsDeleted");

      graph = graph.Save();
      changed = new List<string>();
      graph.PropertyChanged += (o, e) =>
      {
        changed.Add(e.PropertyName);
      };

      Assert.IsFalse(graph.IsDirty, "IsDirty should be false");

      graph.Name = "def";

      Assert.IsTrue(graph.IsDirty, "IsDirty should be true");

      Assert.IsTrue(changed.Contains("Name"), "Name after save");
      Assert.IsTrue(changed.Contains("IsDirty"), "IsDirty after save");
      Assert.IsTrue(changed.Contains("IsSelfDirty"), "IsSelfDirty after save");
      Assert.IsTrue(changed.Contains("IsValid"), "IsValid after save");
      Assert.IsTrue(changed.Contains("IsSelfValid"), "IsSelfValid after save");
      Assert.IsTrue(changed.Contains("IsSavable"), "IsSavable after save");
      Assert.IsFalse(changed.Contains("IsNew"), "IsNew after save");
      Assert.IsFalse(changed.Contains("IsDeleted"), "IsDeleted after save");
    }

    [TestMethod]
    public void RootChangedMetastateEventsChild()
    {
      IChildDataPortal<Child> childDataPortal = _testDIContext.CreateChildDataPortal<Child>();

      Csla.ApplicationContext.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Xaml;
      var graph = NewRoot();
      var changed = new List<string>();
      graph.PropertyChanged += (o, e) =>
      {
        changed.Add(e.PropertyName);
      };
      graph.Name = "abc";
      changed.Clear();
      graph.Children.Add(childDataPortal.FetchChild(123, "xyz"));

      Assert.IsTrue(graph.IsDirty, "IsDirty should be true");

      Assert.IsFalse(changed.Contains("Children"), "Children after add");
      Assert.IsTrue(changed.Contains("IsDirty"), "IsDirty after add");
      Assert.IsFalse(changed.Contains("IsSelfDirty"), "IsSelfDirty after add");
      Assert.IsTrue(changed.Contains("IsValid"), "IsValid after add");
      Assert.IsFalse(changed.Contains("IsSelfValid"), "IsSelfValid after add");
      Assert.IsTrue(changed.Contains("IsSavable"), "IsSavable after add");
      Assert.IsFalse(changed.Contains("IsNew"), "IsNew after add");
      Assert.IsFalse(changed.Contains("IsDeleted"), "IsDeleted after add");

      graph = graph.Save();
      changed.Clear();
      graph.PropertyChanged += (o, e) =>
      {
        changed.Add(e.PropertyName);
      };

      Assert.IsFalse(graph.IsDirty, "IsDirty should be false");

      graph.Children[0].Name = "mnop";

      Assert.IsTrue(graph.IsDirty, "IsDirty should be true");

      Assert.IsFalse(changed.Contains("Children"), "Children after add");
      Assert.IsTrue(changed.Contains("IsDirty"), "IsDirty after add");
      Assert.IsFalse(changed.Contains("IsSelfDirty"), "IsSelfDirty after add");
      Assert.IsTrue(changed.Contains("IsValid"), "IsValid after add");
      Assert.IsFalse(changed.Contains("IsSelfValid"), "IsSelfValid after add");
      Assert.IsTrue(changed.Contains("IsSavable"), "IsSavable after add");
      Assert.IsFalse(changed.Contains("IsNew"), "IsNew after add");
      Assert.IsFalse(changed.Contains("IsDeleted"), "IsDeleted after add");
    }

    private Root NewRoot()
    {
      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();

      return dataPortal.Create();
    }
  }
}
