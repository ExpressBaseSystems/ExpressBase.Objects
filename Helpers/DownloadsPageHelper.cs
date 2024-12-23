﻿using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.ServiceStack.ReqNRes;
using ExpressBase.Common.Structures;
using ExpressBase.Security;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace ExpressBase.Objects.Helpers
{
    public class DownloadsPageHelper
    {
        public int InsertDownloadFileEntry(IDatabase Datadb, String filename, int userId)
        {
            int id = 0;
            try
            {
                string s = $@"INSERT INTO 
                            eb_downloads(filename, eb_created_by, eb_created_at) 
                        VALUES ( :filename, :eb_created_by, {Datadb.EB_CURRENT_TIMESTAMP}) RETURNING id";

                DbParameter[] parameters = new DbParameter[] {
                    Datadb.GetNewParameter("filename", EbDbTypes.String, filename) ,
                    Datadb.GetNewParameter("eb_created_by", EbDbTypes.Int32, userId) ,
                };
                id = Datadb.ExecuteScalar<Int32>(s, parameters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
            }
            return id;
        }

        public (int, string) InsertDownloadFileEntry(IDatabase Datadb, string filename, int userId, byte[] filebytea, bool isPdf)
        {
            try
            {
                string s = $@"INSERT INTO 
                            eb_downloads(filename, eb_created_by, eb_created_at, bytea) 
                        VALUES ( :filename, :eb_created_by, {Datadb.EB_CURRENT_TIMESTAMP}, :bytea) RETURNING id";

                DbParameter[] parameters = new DbParameter[] {
                    Datadb.GetNewParameter("filename", EbDbTypes.String, filename),
                    Datadb.GetNewParameter("eb_created_by", EbDbTypes.Int32, userId),
                    Datadb.GetNewParameter("bytea", EbDbTypes.Bytea, filebytea)
                };
                int id = Datadb.ExecuteScalar<int>(s, parameters);
                if (isPdf)
                    return (id, "/DV/GetPdf?id=" + id);
                return (id, "/DV/GetExcel?id=" + id);
            }
            catch (Exception e)
            {
                return (-1, e.Message);
            }
        }

        public int SaveDownloadFileBytea(IDatabase Datadb, byte[] filebytea, int id)
        {
            try
            {
                string s = @"UPDATE
                            eb_downloads SET bytea = :bytea WHERE id = :id";

                DbParameter[] parameters = new DbParameter[] {
                    Datadb.GetNewParameter("id", EbDbTypes.Int32, id) ,
                    Datadb.GetNewParameter("bytea", EbDbTypes.Bytea, filebytea)
                };
                Datadb.DoNonQuery(s, parameters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
            }
            return id;
        }

        public FileDownloadObject GetDownloadFile(IDatabase Datadb, int Id)
        {
            FileDownloadObject FileDownloadObject = null;
            try
            {
                string s = @"SELECT filename, bytea, eb_created_by, eb_created_at FROM
                            eb_downloads WHERE id=:id AND eb_del = 'F' ";

                DbParameter[] parameters = new DbParameter[] {
                    Datadb.GetNewParameter(":id", EbDbTypes.Int32,  Id) ,
                };
                EbDataTable dt = Datadb.DoQuery(s, parameters);
                if (dt.Rows.Count > 0)
                {
                    FileDownloadObject = new FileDownloadObject
                    {
                        Id = Id,
                        Filename = dt.Rows[0][0].ToString(),
                        FileBytea = (byte[])dt.Rows[0][1],
                        CreatedBy = Convert.ToInt32(dt.Rows[0][2]),
                        //CreatedAt = Convert.ToDateTime(dt.Rows[0][3]),
                    };

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
            }
            return FileDownloadObject;
        }

        public List<FileDownloadObject> GetAllDownloadFiles(IDatabase Datadb, int createdBy, Preferences preference)
        {
            List<FileDownloadObject> FileDownloadObjects = new List<FileDownloadObject>(); ;
            try
            {
                string s = @"SELECT id, filename, eb_created_by, eb_created_at, eb_del, bytea isnull as is_generating FROM
                            eb_downloads WHERE eb_created_by = :createdBy ORDER BY id DESC LIMIT 25";

                DbParameter[] parameters = new DbParameter[] {
                    Datadb.GetNewParameter(":createdBy", EbDbTypes.Int32, createdBy) ,
                };
                EbDataTable dt = Datadb.DoQuery(s, parameters);
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DateTime _date = Convert.ToDateTime(dt.Rows[i][3]);
                        FileDownloadObjects.Add(new FileDownloadObject
                        {
                            Id = Convert.ToInt32(dt.Rows[i][0]),
                            Filename = dt.Rows[i][1].ToString(),
                            CreatedBy = Convert.ToInt32(dt.Rows[i][2]),
                            CreatedAt = (_date).ConvertFromUtc(preference.TimeZone).ToString(preference.GetShortDatePattern()) + " " + _date.ConvertFromUtc(preference.TimeZone).ToString(preference.GetShortTimePattern()),
                            IsDeleted = ((char)dt.Rows[i][4] == 'F') ? false : true,
                            IsGenerating = (bool)dt.Rows[i][5],
                        });

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
            }
            return FileDownloadObjects;
        }

        public bool DeleteDownloadFile(IDatabase Datadb, int Id)
        {
            bool status = false;
            try
            {
                string s = @"UPDATE
                                eb_downloads 
                            SET 
                                bytea = NULL, eb_del = 'T' 
                            WHERE id = :id ";

                DbParameter[] parameters = new DbParameter[] {
                    Datadb.GetNewParameter(":id", EbDbTypes.Int32, Id)
                };

                int id = Datadb.DoNonQuery(s, parameters);
                if (id > 0)
                    status = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
            }
            return status;
        }

    }
}
