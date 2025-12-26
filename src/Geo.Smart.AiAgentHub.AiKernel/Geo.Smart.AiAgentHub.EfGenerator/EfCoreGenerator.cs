using Efrpg;
using Efrpg.FileManagement;
using Efrpg.Filtering;
using Efrpg.Generators;
using Efrpg.Pluralization;
using Efrpg.Templates;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace Geo.Smart.AiAgentHub.EfGenerator
{
    public static class EfCoreGenerator
    {
        public static void Generate()
        {
            const string ns = "Geo.Smart.AiAgentHub";
            // 要產生 Code First 程式碼的根目錄路徑
            var root = $"../../../{ns}.DataAccess";
            Settings.Root = Path.Combine(root, "");

            // 設定專案的 Namespace
            var nameSpace = $"{ns}.DataAccess";

            // 設定資料庫連線字串
            var connectionString =
                @"Data Source=DevDb4\Gdb2022; Initial Catalog=AiAgentHub_Schema; Integrated Security=True;Application Name=Generator;";
            if (!TryConnect(connectionString))
            {
                throw new ArgumentException("資料庫連線失敗！請重新檢查。");
            }

            SetupDatabase(connectionString, nameSpace);
            Run();
        }

        private static void Run()
        {
            FilterSettings.CheckSettings();
            Inflector.PluralisationService = new EnglishPluralizationService();

            var outer = new GeneratedTextTransformation();
            var fileManagement = new FileManagementService(outer);
            Generator generator = GeneratorFactory.Create(fileManagement, FileManagerFactory.GetFileManagerType());
            if (generator != null && generator.InitialisationOk)
            {
                generator.ReadDatabase();
                generator.GenerateCode();
            }

            fileManagement.Process(true);
        }

        private static void SetupDatabase(
            string connectionString, string nameSpace)
        {
            Settings.DatabaseType = DatabaseType.SqlServer;
            Settings.TemplateType = TemplateType.FileBasedCore8;
            Settings.GeneratorType = GeneratorType.EfCore;

            Settings.FileManagerType = FileManagerType.EfCore;
            Settings.ConnectionString = connectionString;
            Settings.ConnectionStringName = "GdbConnection";
            Settings.DbContextName = "GdbContext";
            Settings.GenerateSeparateFiles = true;
            Settings.Namespace = nameSpace;
            Settings.TemplateFolder = "./Templates.EFCore8";
            Settings.AddUnitTestingDbContext = false;

            ResetFilters();

            Settings.ElementsToGenerate = Elements.Poco | Elements.Context | Elements.Interface | Elements.PocoConfiguration | Elements.Enum;
            if (Settings.GenerateSeparateFiles && Settings.FileManagerType == FileManagerType.EfCore)
            {
                Settings.ContextFolder = @"";
                Settings.InterfaceFolder = @"Interface";
                Settings.PocoFolder = @"Entities";
                Settings.PocoConfigurationFolder = @"Configuration";
            }

            // Other settings
            Settings.CommandTimeout = 600;
            Settings.DbContextInterfaceBaseClasses = "IDisposable";
            Settings.DbContextBaseClass = "IdentityDbContext<ApplicationUser, ApplicationRole, string, ApplicationUserClaim, IdentityUserRole<string>, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>";
            Settings.OnConfiguration = OnConfiguration.ConnectionString;
            Settings.AddParameterlessConstructorToDbContext = false;
            Settings.ConfigurationClassName = "Configuration";
            Settings.DatabaseReaderPlugin = "";
            // 這是多對多的設定，UseMappingTables = true 不會會自動產生中介表 "Rel*"
            Settings.UseMappingTables = true;

            Settings.EntityClassesModifiers = "public";
            Settings.ConfigurationClassesModifiers = "public";
            Settings.DbContextClassModifiers = "public partial";
            Settings.DbContextInterfaceModifiers = "public partial";
            Settings.ResultClassModifiers = "public";

            Settings.DisableGeographyTypes = false;
            Settings.UsePascalCase = true;
            Settings.PrependSchemaName = false;

            Settings.UsePropertyInitialisers = true;
            Settings.UseLazyLoading = false;
            Settings.IncludeComments = CommentsStyle.None;
            Settings.IncludeExtendedPropertyComments = CommentsStyle.InSummaryBlock;
            Settings.DisableGeographyTypes = false;
            Settings.NullableShortHand = true;
            Settings.AddIDbContextFactory = false;

            Settings.AdditionalNamespaces = new List<string>();
            Settings.AdditionalContextInterfaceItems = new List<string>();
            Settings.AdditionalFileHeaderText = new List<string>();
            Settings.AdditionalFileFooterText = new List<string>();
            Settings.AllowNullStrings = true;

            Settings.Enumerations = new List<EnumerationSettings>
            {
                // 暫時用不到
            };

            Settings.AddEnumDefinitions = delegate (List<EnumDefinition> enumDefinitions)
            {
                enumDefinitions.Add(new EnumDefinition
                {
                    Schema = Settings.DefaultSchema,
                    Table = "ApplicationUser",
                    Column = "LoginType",
                    EnumType = "LoginType"
                });
                enumDefinitions.Add(new EnumDefinition
                {
                    Schema = Settings.DefaultSchema,
                    Table = "UserHistory",
                    Column = "UserHistoryType",
                    EnumType = "UserHistoryType"
                });
                enumDefinitions.Add(new EnumDefinition
                {
                    Schema = Settings.DefaultSchema,
                    Table = "VerifyCode",
                    Column = "VerifyType",
                    EnumType = "VerifyType"
                });

                enumDefinitions.Add(new EnumDefinition
                {
                    Schema = Settings.DefaultSchema,
                    Table = "LlmInfo",
                    Column = "LlmSourceType",
                    EnumType = "LlmSourceType"
                });
                enumDefinitions.Add(new EnumDefinition
                {
                    Schema = Settings.DefaultSchema,
                    Table = "McpServer",
                    Column = "McpServerType",
                    EnumType = "McpServerType"
                });
            };
            Settings.TableRename = delegate (string name, string schema, bool isView)
            {
                if (name == "AspNetUsers")
                {
                    return "ApplicationUser";
                }
                else if (name == "AspNetRoles")
                {
                    return "ApplicationRole";
                }
                else if (name == "AspNetUserRoles")
                {
                    return "ApplicationUserRole";
                }
                else if (name == "AspNetUserClaims")
                {
                    return "ApplicationUserClaim";
                }
                else if (name == "AspNetRoleClaims")
                {
                    return "ApplicationRoleClaim";
                }
                else if (name == "AspNetUserLogins")
                {
                    return "ApplicationUserLogin";
                }
                else if (name == "AspNetUserTokens")
                {
                    return "ApplicationUserToken";
                }
                else if (name == "Files")
                {
                    return "Filex";
                }
                else if (name == "UserToken")
                {
                    return "UserTokenx";
                }

                return name;
            };
            Settings.UpdateTable = delegate (Table table)
            {
                if (table.DbName == "AspNetUsers")
                {
                    table.BaseClasses = " : IdentityUser";
                    var userInBase = new List<string>
                    {
                        "id","username","normalizedusername","email","normalizedemail",
                        "emailconfirmed","passwordhash","securitystamp","concurrencystamp",
                        "phonenumber","phonenumberconfirmed","twofactorenabled","lockoutend",
                        "lockoutenabled","accessfailedcount"
                    };
                    ExcludeInBase(table, userInBase);
                }
                if (table.DbName == "AspNetRoles")
                {
                    table.BaseClasses = " : IdentityRole<string>";
                    ExcludeInBase(table, new List<string>
                    {
                        "id","name","normalizedname","concurrencystamp"
                    });
                }
                if (table.DbName == "AspNetRoleClaims")
                {
                    table.BaseClasses = " : IdentityRoleClaim<string>";
                    ExcludeInBase(table, new List<string>
                    {
                        "id","roleid","claimtype","claimvalue"
                    });
                }

                if (table.DbName == "AspNetUserClaims")
                {
                    table.BaseClasses = " : IdentityUserClaim<string>";
                    ExcludeInBase(table, new List<string>
                    {
                        "id","userid","claimtype","claimvalue"
                    });
                }

                if (table.DbName == "AspNetUserLogins")
                {
                    table.BaseClasses = " : IdentityUserLogin<string>";
                    ExcludeInBase(table, new List<string>
                    {
                        "loginprovider","providerkey","providerdisplayname","userid"
                    });
                }
                if (table.DbName == "AspNetUserTokens")
                {
                    table.BaseClasses = " : IdentityUserToken<string>";
                    ExcludeInBase(table, new List<string>
                    {
                        "userid","loginprovider","name","value"
                    });
                }
                if (table.DbName == "AspNetUserRoles")
                {
                    table.BaseClasses = " : IdentityUserRole<string>";
                    ExcludeInBase(table, new List<string>
                    {
                        "userid","roleid"
                    });
                }

                var audits = new List<string> {
                    "createdby","createddate","updatedby","updateddate", "isenabled"
                };
                var columns = table.Columns.Select(x => x.NameHumanCase.ToLower());
                if (audits.TrueForAll(x => columns.Contains(x)))
                {
                    table.BaseClasses = " : AuditableEntity";
                    ExcludeInBase(table, audits);
                }
            };

            Settings.GenerateSingleDbContext = true;
            Settings.MultiContextSettingsConnectionString = "";
            Settings.MultiContextSettingsPlugin = "";
            Settings.MultiContextAttributeDelimiter = '~';
        }

        /// <summary>
        /// 當有繼承時，需要繼承的欄位
        /// </summary>
        /// <param name="table"></param>
        /// <param name="excludes"></param>
        private static void ExcludeInBase(Table table, List<string> excludes)
        {
            table.Columns.ForEach(c =>
            {
                if (excludes.Contains(c.NameHumanCase.ToLower()))
                    c.ExistsInBaseClass = true;
            });
        }

        private static void ResetFilters()
        {
            FilterSettings.Reset();
            FilterSettings.AddDefaults();

            FilterSettings.IncludeTableValuedFunctions = true;
            FilterSettings.TableFilters.Add(new RegexExcludeFilter("__EFMigrationsHistory"));
            FilterSettings.TableFilters.Add(new RegexExcludeFilter("ChatCompletionLog"));
            FilterSettings.TableFilters.Add(new RegexExcludeFilter("ChatMessage"));
            FilterSettings.TableFilters.Add(new RegexExcludeFilter("ChatRoom"));
            FilterSettings.TableFilters.Add(new RegexExcludeFilter("Nlog"));
            FilterSettings.TableFilters.Add(new RegexExcludeFilter("Elmah"));
            FilterSettings.TableFilters.Add(new RegexExcludeFilter(".*geometry_columns.*"));
            FilterSettings.TableFilters.Add(new RegexExcludeFilter(".*spatial_ref_sys.*"));
        }

        private static bool TryConnect(string cs)
        {
            var conn = new SqlConnection(cs);
            try
            {
                conn.Open();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}