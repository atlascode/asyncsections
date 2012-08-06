using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.Reflection;

namespace AtlasCode.AsyncSections
{
	/// <summary>
	/// Taken from http://exposedobject.codeplex.com/
	/// This is used to make internal reflection easier in AsyncSectionController.cs
	/// Ideally this could get merged into the next major MVC version :-) and the need for all the reflection is removed.
	/// </summary>
    public class ExposedObject : DynamicObject
    {
        private object m_object;

        public ExposedObject(object obj)
        {
            m_object = obj;
        }

        public override bool TryInvokeMember(
                InvokeMemberBinder binder, object[] args, out object result)
        {
            // Find the called method using reflection
            var methodInfo = m_object.GetType().GetMethod(binder.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (methodInfo == null)
            {
                var propertyInfo = m_object.GetType().GetProperty(binder.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                System.Delegate method = propertyInfo.GetValue(m_object, new object[] { }) as System.Delegate;

                result = method.Method.Invoke(m_object, args);
            }
            else
            {

                // Call the method
                result = methodInfo.Invoke(m_object, args);
            }

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var propertyInfo = m_object.GetType().GetProperty(
                binder.Name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (propertyInfo != null)
            {
                propertyInfo.SetValue(m_object, value, null);
                return true;
            }

            var fieldInfo = m_object.GetType().GetField(
                binder.Name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (fieldInfo != null)
            {
                fieldInfo.SetValue(m_object, value);
                return true;
            }

            return false;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var propertyInfo = m_object.GetType().GetProperty(
                binder.Name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (propertyInfo != null)
            {
                result = propertyInfo.GetValue(m_object, null);
                return true;
            }

            var fieldInfo = m_object.GetType().GetField(
                binder.Name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (fieldInfo != null)
            {
                result = fieldInfo.GetValue(m_object);
                return true;
            }

            result = null;
            return false;
        }
    }
}