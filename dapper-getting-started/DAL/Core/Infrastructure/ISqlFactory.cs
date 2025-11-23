using Microsoft.Data.SqlClient;

namespace DAL.Core.Infrastructure;

/// <summary>
/// Provides factory methods for creating <see cref="SqlConnection"/> and <see cref="SqlCommand"/>
/// instances used throughout the <c>mcsCore</c> repositories layer.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="ISqlFactory"/> centralizes construction of ADO.NET artifacts so that connection string
/// formatting, security options, and stored procedure configuration remain consistent across the
/// repositories layer.
/// </para>
///
/// <para>
/// The factory supports three primary connection scenarios:
/// </para>
///
/// <list type="number">
///   <item>
///     <description>
///       <b>Configuration-based connection</b>:
///       <see cref="BuildSqlConnection(string?)"/> builds a <see cref="SqlConnection"/> using a
///       connection string from <see cref="ConfigurationManager.AppSettings"/>. When
///       <paramref name="settingName"/> is <see langword="null"/>, the <c>"connectionString"</c>
///       key is used.
///     </description>
///   </item>
///   <item>
///     <description>
///       <b>Windows Authentication (Integrated Security)</b>:
///       <see cref="BuildSqlConnection(string, string, bool, bool)"/> builds a connection string
///       using the specified SQL Server <c>server</c> and <c>database</c> with 
///       <c>IntegratedSecurity=true</c>. Username and password are not allowed in this mode.
///     </description>
///   </item>
///   <item>
///     <description>
///       <b>SQL Server Authentication</b>:
///       <see cref="BuildSqlConnection(string, string, string, string, bool, bool)"/> builds a
///       connection string using explicit <c>username</c> and <c>password</c> with
///       <c>IntegratedSecurity = false</c>. Both values are required.
///     </description>
///   </item>
/// </list>
///
/// <para>
/// The <paramref name="encrypt"/> and <paramref name="trustCertificate"/> parameters map directly
/// to <see cref="SqlConnectionStringBuilder.Encrypt"/> and
/// <see cref="SqlConnectionStringBuilder.TrustServerCertificate"/>. Note that
/// <c>Microsoft.Data.SqlClient</c> defaults to <c>Encrypt = true</c>, whereas
/// <c>System.Data.SqlClient</c> historically defaulted to <c>Encrypt = false</c>, so existing
/// connection strings may behave differently when migrated.
/// </para>
///
/// <para>
/// When <c>IntegratedSecurity = true</c>, specifying <c>username</c> or <c>password</c> results in
/// an <see cref="ArgumentException"/>. When using SQL Server Authentication, both
/// <c>username</c> and <c>password</c> are required.
/// </para>
///
/// <para>
/// The command factory methods (<see cref="BuildSqlCommand(string, GPEvent)"/>,
/// <see cref="BuildSqlCommand(string, SqlConnection)"/>,
/// <see cref="BuildSqlCommand(string, SqlConnection, SqlTransaction)"/>) standardize stored
/// procedure execution by:
/// </para>
///
/// <list type="bullet">
///   <item>
///     <description>Setting <see cref="SqlCommand.CommandType"/> to <see cref="CommandType.StoredProcedure"/>.</description>
///   </item>
///   <item>
///     <description>
///       Applying a configurable command timeout, read from <c>AppSettings["commandTimeOut"]</c>
///       (defaulting to 30 seconds when not specified).
///     </description>
///   </item>
/// </list>
///
/// <para>
/// This comment is intentionally compact for IntelliSense. A more detailed design discussion and
/// usage examples are documented in the implementation (<see cref="SqlFactory"/>) and in the
/// project documentation.
/// </para>
/// </remarks>
public interface ISqlFactory
{
    SqlConnection BuildSqlConnection(string? settingName = null);
    SqlConnection BuildSqlConnection(string server, string database, bool encrypt = true, bool trustCertificate = false);
    SqlConnection BuildSqlConnection(string server, string database, string username, string password, bool encrypt = true, bool trustCertificate = false);

    SqlCommand BuildSqlCommand(string procedureName, SqlConnection connection);
    SqlCommand BuildSqlCommand(string procedureName, SqlConnection connection, SqlTransaction transaction);
}
