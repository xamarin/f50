﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Database;
using Android.Provider;
using Net = Android.Net;

namespace Xamarin.Essentials
{
    public static partial class Contact
    {
        static Activity Activity => Platform.GetCurrentActivity(true);

        internal static Action<PhoneContact> CallBack { get; set; }

        static Task<PhoneContact> PlataformPickContactAsync()
        {
            var source = new TaskCompletionSource<PhoneContact>();
            try
            {
                var contactPicker = new Intent(Activity, typeof(ContactActivity));
                contactPicker.SetFlags(ActivityFlags.NewTask);
                Activity.StartActivity(contactPicker);

                CallBack = (phoneContact) =>
                {
                    var tcs = Interlocked.Exchange(ref source, null);
                    tcs?.SetResult(phoneContact);
                };
            }
            catch (Exception ex)
            {
                source.SetException(ex);
            }

            return source.Task;
        }

        internal static PhoneContact PlataformGetContacts(Net.Uri contactUri)
        {
            var context = Activity.ContentResolver;

            var cur = context.Query(contactUri, null, null, null, null);
            var emails = new Dictionary<string, ContactType>();
            var phones = new Dictionary<string, ContactType>();
            var bDate = string.Empty;

            if (cur.MoveToFirst())
            {
                var name = cur.GetString(cur.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.DisplayName));
                string id;

                id = cur.GetString(cur.GetColumnIndex(ContactsContract.CommonDataKinds.Email.InterfaceConsts.ContactId));

                var idQ = new string[] { id };

                var projection = new string[]
                {
                        ContactsContract.CommonDataKinds.Phone.Number,
                        ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Type,
                };

                var cursor = context.Query(
                    ContactsContract.CommonDataKinds.Phone.ContentUri,
                    null,
                    ContactsContract.CommonDataKinds.Phone.InterfaceConsts.ContactId + " = ?",
                    idQ,
                    null);

                if (cursor.MoveToFirst())
                {
                    do
                    {
                        var phone = cursor.GetString(cursor.GetColumnIndex(projection[0]));
                        var phoneType = cursor.GetString(cursor.GetColumnIndex(projection[1]));

                        var contactType = GetContactType(phoneType);
                        if (!phones.ContainsKey(phone))
                            phones.Add(phone, contactType);
                    }
                    while (cursor.MoveToNext());
                }
                cursor.Close();

                projection = new string[]
                {
                        ContactsContract.CommonDataKinds.Email.Address,
                        ContactsContract.CommonDataKinds.Email.InterfaceConsts.Type
                };

                cursor = context.Query(ContactsContract.CommonDataKinds.Email.ContentUri, null, ContactsContract.CommonDataKinds.Email.InterfaceConsts.ContactId + " = ?", idQ, null);

                while (cursor.MoveToNext())
                {
                    var email = cursor.GetString(cursor.GetColumnIndex(projection[0]));
                    var emailType = cursor.GetString(cursor.GetColumnIndex(projection[1]));

                    var contactType = GetContactType(emailType);
                    if (!emails.ContainsKey(email))
                        emails.Add(email, contactType);
                }

                cursor.Close();

                projection = new string[]
                {
                            ContactsContract.CommonDataKinds.StructuredPostal.Street,
                            ContactsContract.CommonDataKinds.StructuredPostal.City,
                            ContactsContract.CommonDataKinds.StructuredPostal.Postcode
                };

                cursor = context.Query(ContactsContract.CommonDataKinds.StructuredPostal.ContentUri, projection, ContactsContract.Data.InterfaceConsts.ContactId + " = ?", idQ, null);
                if (cursor.MoveToLast())
                {
                    // Add street in PhoneContact struct

                    var street = cursor.GetString(cursor.GetColumnIndex(projection[0]));
                    var city = cursor.GetString(cursor.GetColumnIndex(projection[1]));
                    var postCode = cursor.GetString(cursor.GetColumnIndex(projection[2]));
                }
                cursor.Close();

                var query = ContactsContract.CommonDataKinds.CommonColumns.Type + " = " + 3
                     + " AND " + ContactsContract.CommonDataKinds.Event.InterfaceConsts.ContactId + " = ?";

                cursor = context.Query(ContactsContract.Data.ContentUri, null, query, idQ, null);
                if (cursor.MoveToFirst())
                {
                    bDate = cursor.GetString(cursor.GetColumnIndex(ContactsContract.CommonDataKinds.Event.StartDate));
                }
                cursor.Close();
                DateTime.TryParse(bDate, out var birthday);
                return new PhoneContact(name, phones, emails, birthday);
            }

            return default;
        }

        static Task PlataformSaveContactAsync(string name, string phone, string email)
        {
            var intent = new Intent(Intent.ActionInsert);
            intent.SetType(ContactsContract.Contacts.ContentType);
            intent.PutExtra(ContactsContract.Intents.Insert.Name, name);
            intent.PutExtra(ContactsContract.Intents.Insert.Phone, phone);
            intent.PutExtra(ContactsContract.Intents.Insert.Email, email);
            Activity.StartActivity(intent);

            return Task.CompletedTask;
        }

        static ContactType GetContactType(string type)
        {
            if (type == "1" || type == "2" || type == "5")
                return ContactType.Personal;
            else if (type == "3" || type == "17" || type == "18")
                return ContactType.Work;
            else
                return ContactType.Unknow;
        }

        static Task PlataformSaveContact(PhoneContact phoneContact) => Task.CompletedTask;
    }
}
