using System.Collections;
using System.Collections.Generic;

namespace Csla.Core
{

  public abstract partial class BusinessBase
  {
    [Csla.NotUndoableAttribute()]
    private Hashtable m_OriginalState = null;
    [Csla.NotUndoableAttribute()]
    private string m_Edits = string.Empty;

    /// <summary>
    /// Edits on the object.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public string Edits
    {
      get { return m_Edits; }
      set { m_Edits = value; }
    }

    private void SetEdits()
    {
      m_OriginalState = new Hashtable();
      m_Edits = string.Empty;
      foreach (Core.IPropertyInfo info in this.FieldManager.GetRegisteredProperties())
      {
        var value = this.GetProperty(info);
        if (value is object)
        {
          m_OriginalState.Add(info.Name, value.ToString());
        }
      }
    }

    private void GetEdits()
    {
      var sb = new System.Text.StringBuilder();
      if (m_OriginalState is object)
      {
        foreach (Core.IPropertyInfo info in this.FieldManager.GetRegisteredProperties())
        {
          var value = this.GetProperty(info);
          if (value is object)
          {
            if (m_OriginalState.ContainsKey(info.Name))
            {
              string oldvalue = m_OriginalState[info.Name].ToString();
              if ((oldvalue ?? "") != (value.ToString() ?? ""))
              {
                sb.Append(info.Name + "\t" + value.ToString() + "\t" + oldvalue + "\t");
              }
            }
          }
        }
      }
      m_Edits = sb.ToString();
    }

    private void UndoEdits()
    {
      m_OriginalState = null;
      m_Edits = string.Empty;
    }

    /// <summary>
    /// Tab delimited string of properties on the object.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public string ShowMembers(string PrependValue)
    {
      var sb = new System.Text.StringBuilder();
      foreach (Core.IPropertyInfo info in this.FieldManager.GetRegisteredProperties())
      {
        var value = this.GetProperty(info);
        if (value is object)
        {
          sb.Append(info.Name + "\t" + value.ToString() + "\t");
        }
      }

      return sb.ToString();
    }

    private struct EditFieldInfo
    {
      public string FieldName;
      public string OldValue;
      public string NewValue;
    }

    /// <summary>
    /// Process edits so each edited field is only returned once, with its most recent and original values.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public string MergeEdits(string value)
    {
      string[] a = value.Split('\t');
      var f = new List<string>();
      var e = new List<EditFieldInfo>();
      for (int i = 0, loopTo = a.Length - 2; i <= loopTo; i += 3)
      {
        if (f.Contains(a[i]))
        {
          foreach (EditFieldInfo o in e)
          {
            if ((o.FieldName ?? "") == (a[i] ?? ""))
            {
              var n = o;
              n.NewValue = a[i + 1];
              e.Remove(o);
              if ((o.OldValue ?? "") != (n.NewValue ?? ""))
              {
                e.Add(n);
              }
              break;
            }
          }
        }
        else
        {
          f.Add(a[i]);
          var ed = new EditFieldInfo();
          ed.FieldName = a[i];
          ed.NewValue = a[i + 1];
          ed.OldValue = a[i + 2];
          e.Add(ed);
        }
      }

      string s = "";
      foreach (EditFieldInfo o in e)
        s += o.FieldName + "\t" + o.NewValue + "\t" + o.OldValue + "\t";
      return s;
    }

    private void KillCache()
    {
      _readResultCache = null;
    }
  }
}