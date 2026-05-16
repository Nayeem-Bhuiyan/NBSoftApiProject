-- Create Database if not exists
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'NBSoftApiDB')
BEGIN
    CREATE DATABASE [NBSoftApiDB];
    PRINT 'Database NBSoftApiDB created.';
END
ELSE
    PRINT 'Database NBSoftApiDB already exists.';
GO

USE [NBSoftApiDB];  -- ← FIXED: Changed from NBSoftÄpiDB
GO

-- Create Login and User
IF NOT EXISTS (SELECT * FROM sys.sql_logins WHERE name = 'AppUser')
BEGIN
    CREATE LOGIN [AppUser] WITH PASSWORD = 'App@User123!', 
        CHECK_EXPIRATION=OFF, 
        CHECK_POLICY=OFF;
    
    CREATE USER [AppUser] FOR LOGIN [AppUser];
    ALTER ROLE [db_owner] ADD MEMBER [AppUser];
    
    PRINT 'User AppUser created and granted db_owner permission.';
END
ELSE
    PRINT 'User AppUser already exists.';
GO

-- Wait for Country table to be created by EF migrations
PRINT '=== Waiting for Country table to be created ===';
DECLARE @Counter INT = 0;
DECLARE @TableExists INT = 0;

WHILE @Counter < 30 AND @TableExists = 0
BEGIN
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Country' AND type = 'U')
    BEGIN
        SET @TableExists = 1;
        PRINT 'Country table found after ' + CAST(@Counter AS VARCHAR) + ' seconds';
    END
    ELSE
    BEGIN
        WAITFOR DELAY '00:00:02';
        SET @Counter = @Counter + 2;
    END
END

IF @TableExists = 0
BEGIN
    PRINT '⚠️ Country table not found. Migrations may not have run yet.';
    PRINT 'Will continue but data insertion may fail.';
END
ELSE
BEGIN
    PRINT '=== Clearing existing country data ===';
    DELETE FROM dbo.Country;
    
    PRINT '=== Inserting countries ===';
    
    INSERT INTO dbo.Country 
    ([Name], Code, Continent, Currency, PhoneCode, IsActive, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsDeleted, DeletedBy, DeletedDate)
VALUES
('Afghanistan', 'AF', 'Asia', 'AFN', '+93', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Albania', 'AL', 'Europe', 'ALL', '+355', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Algeria', 'DZ', 'Africa', 'DZD', '+213', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Andorra', 'AD', 'Europe', 'EUR', '+376', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Angola', 'AO', 'Africa', 'AOA', '+244', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Antigua and Barbuda', 'AG', 'North America', 'XCD', '+1-268', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Argentina', 'AR', 'South America', 'ARS', '+54', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Armenia', 'AM', 'Asia', 'AMD', '+374', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Australia', 'AU', 'Oceania', 'AUD', '+61', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Austria', 'AT', 'Europe', 'EUR', '+43', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Azerbaijan', 'AZ', 'Asia', 'AZN', '+994', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Bahamas', 'BS', 'North America', 'BSD', '+1-242', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Bahrain', 'BH', 'Asia', 'BHD', '+973', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Bangladesh', 'BD', 'Asia', 'BDT', '+880', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Barbados', 'BB', 'North America', 'BBD', '+1-246', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Belarus', 'BY', 'Europe', 'BYN', '+375', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Belgium', 'BE', 'Europe', 'EUR', '+32', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Belize', 'BZ', 'North America', 'BZD', '+501', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Benin', 'BJ', 'Africa', 'XOF', '+229', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Bhutan', 'BT', 'Asia', 'BTN', '+975', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Bolivia', 'BO', 'South America', 'BOB', '+591', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Bosnia and Herzegovina', 'BA', 'Europe', 'BAM', '+387', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Botswana', 'BW', 'Africa', 'BWP', '+267', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Brazil', 'BR', 'South America', 'BRL', '+55', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Brunei', 'BN', 'Asia', 'BND', '+673', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Bulgaria', 'BG', 'Europe', 'BGN', '+359', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Burkina Faso', 'BF', 'Africa', 'XOF', '+226', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Burundi', 'BI', 'Africa', 'BIF', '+257', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Cabo Verde', 'CV', 'Africa', 'CVE', '+238', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Cambodia', 'KH', 'Asia', 'KHR', '+855', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Cameroon', 'CM', 'Africa', 'XAF', '+237', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Canada', 'CA', 'North America', 'CAD', '+1', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Central African Republic', 'CF', 'Africa', 'XAF', '+236', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Chad', 'TD', 'Africa', 'XAF', '+235', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Chile', 'CL', 'South America', 'CLP', '+56', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('China', 'CN', 'Asia', 'CNY', '+86', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Colombia', 'CO', 'South America', 'COP', '+57', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Comoros', 'KM', 'Africa', 'KMF', '+269', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Congo (Congo-Brazzaville)', 'CG', 'Africa', 'XAF', '+242', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Congo (Democratic Republic)', 'CD', 'Africa', 'CDF', '+243', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Costa Rica', 'CR', 'North America', 'CRC', '+506', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Croatia', 'HR', 'Europe', 'EUR', '+385', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Cuba', 'CU', 'North America', 'CUP', '+53', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Cyprus', 'CY', 'Europe', 'EUR', '+357', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Czech Republic', 'CZ', 'Europe', 'CZK', '+420', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Denmark', 'DK', 'Europe', 'DKK', '+45', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Djibouti', 'DJ', 'Africa', 'DJF', '+253', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Dominica', 'DM', 'North America', 'XCD', '+1-767', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Dominican Republic', 'DO', 'North America', 'DOP', '+1-809', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Ecuador', 'EC', 'South America', 'USD', '+593', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Egypt', 'EG', 'Africa', 'EGP', '+20', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('El Salvador', 'SV', 'North America', 'USD', '+503', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Equatorial Guinea', 'GQ', 'Africa', 'XAF', '+240', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Eritrea', 'ER', 'Africa', 'ERN', '+291', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Estonia', 'EE', 'Europe', 'EUR', '+372', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Eswatini', 'SZ', 'Africa', 'SZL', '+268', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Ethiopia', 'ET', 'Africa', 'ETB', '+251', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Fiji', 'FJ', 'Oceania', 'FJD', '+679', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Finland', 'FI', 'Europe', 'EUR', '+358', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('France', 'FR', 'Europe', 'EUR', '+33', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Gabon', 'GA', 'Africa', 'XAF', '+241', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Gambia', 'GM', 'Africa', 'GMD', '+220', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Georgia', 'GE', 'Asia', 'GEL', '+995', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Germany', 'DE', 'Europe', 'EUR', '+49', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Ghana', 'GH', 'Africa', 'GHS', '+233', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Greece', 'GR', 'Europe', 'EUR', '+30', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Grenada', 'GD', 'North America', 'XCD', '+1-473', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Guatemala', 'GT', 'North America', 'GTQ', '+502', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Guinea', 'GN', 'Africa', 'GNF', '+224', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Guinea-Bissau', 'GW', 'Africa', 'XOF', '+245', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Guyana', 'GY', 'South America', 'GYD', '+592', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Haiti', 'HT', 'North America', 'HTG', '+509', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Honduras', 'HN', 'North America', 'HNL', '+504', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Hungary', 'HU', 'Europe', 'HUF', '+36', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Iceland', 'IS', 'Europe', 'ISK', '+354', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('India', 'IN', 'Asia', 'INR', '+91', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Indonesia', 'ID', 'Asia', 'IDR', '+62', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Iran', 'IR', 'Asia', 'IRR', '+98', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Iraq', 'IQ', 'Asia', 'IQD', '+964', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Ireland', 'IE', 'Europe', 'EUR', '+353', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Israel', 'IL', 'Asia', 'ILS', '+972', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Italy', 'IT', 'Europe', 'EUR', '+39', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Jamaica', 'JM', 'North America', 'JMD', '+1-876', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Japan', 'JP', 'Asia', 'JPY', '+81', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Jordan', 'JO', 'Asia', 'JOD', '+962', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Kazakhstan', 'KZ', 'Asia', 'KZT', '+7', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Kenya', 'KE', 'Africa', 'KES', '+254', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Kiribati', 'KI', 'Oceania', 'AUD', '+686', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Kuwait', 'KW', 'Asia', 'KWD', '+965', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Kyrgyzstan', 'KG', 'Asia', 'KGS', '+996', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Laos', 'LA', 'Asia', 'LAK', '+856', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Latvia', 'LV', 'Europe', 'EUR', '+371', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Lebanon', 'LB', 'Asia', 'LBP', '+961', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Lesotho', 'LS', 'Africa', 'LSL', '+266', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Liberia', 'LR', 'Africa', 'LRD', '+231', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Libya', 'LY', 'Africa', 'LYD', '+218', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Liechtenstein', 'LI', 'Europe', 'CHF', '+423', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Lithuania', 'LT', 'Europe', 'EUR', '+370', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
('Luxembourg', 'LU', 'Europe', 'EUR', '+352', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Madagascar', 'MG', 'Africa', 'MGA', '+261', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Malawi', 'MW', 'Africa', 'MWK', '+265', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Malaysia', 'MY', 'Asia', 'MYR', '+60', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Maldives', 'MV', 'Asia', 'MVR', '+960', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Mali', 'ML', 'Africa', 'XOF', '+223', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Malta', 'MT', 'Europe', 'EUR', '+356', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Marshall Islands', 'MH', 'Oceania', 'USD', '+692', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Mauritania', 'MR', 'Africa', 'MRU', '+222', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Mauritius', 'MU', 'Africa', 'MUR', '+230', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Mexico', 'MX', 'North America', 'MXN', '+52', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Micronesia', 'FM', 'Oceania', 'USD', '+691', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Moldova', 'MD', 'Europe', 'MDL', '+373', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Monaco', 'MC', 'Europe', 'EUR', '+377', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Mongolia', 'MN', 'Asia', 'MNT', '+976', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Montenegro', 'ME', 'Europe', 'EUR', '+382', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Morocco', 'MA', 'Africa', 'MAD', '+212', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Mozambique', 'MZ', 'Africa', 'MZN', '+258', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Myanmar', 'MM', 'Asia', 'MMK', '+95', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Namibia', 'NA', 'Africa', 'NAD', '+264', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Nauru', 'NR', 'Oceania', 'AUD', '+674', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Nepal', 'NP', 'Asia', 'NPR', '+977', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Netherlands', 'NL', 'Europe', 'EUR', '+31', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'New Zealand', 'NZ', 'Oceania', 'NZD', '+64', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Nicaragua', 'NI', 'North America', 'NIO', '+505', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Niger', 'NE', 'Africa', 'XOF', '+227', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Nigeria', 'NG', 'Africa', 'NGN', '+234', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'North Korea', 'KP', 'Asia', 'KPW', '+850', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'North Macedonia', 'MK', 'Europe', 'MKD', '+389', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Norway', 'NO', 'Europe', 'NOK', '+47', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Oman', 'OM', 'Asia', 'OMR', '+968', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Pakistan', 'PK', 'Asia', 'PKR', '+92', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Palau', 'PW', 'Oceania', 'USD', '+680', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Palestine', 'PS', 'Asia', 'ILS', '+970', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Panama', 'PA', 'North America', 'PAB', '+507', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Papua New Guinea', 'PG', 'Oceania', 'PGK', '+675', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Paraguay', 'PY', 'South America', 'PYG', '+595', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Peru', 'PE', 'South America', 'PEN', '+51', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Philippines', 'PH', 'Asia', 'PHP', '+63', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Poland', 'PL', 'Europe', 'PLN', '+48', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Portugal', 'PT', 'Europe', 'EUR', '+351', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Qatar', 'QA', 'Asia', 'QAR', '+974', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Romania', 'RO', 'Europe', 'RON', '+40', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Russia', 'RU', 'Europe', 'RUB', '+7', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Rwanda', 'RW', 'Africa', 'RWF', '+250', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Saint Kitts and Nevis', 'KN', 'North America', 'XCD', '+1-869', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Saint Lucia', 'LC', 'North America', 'XCD', '+1-758', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Saint Vincent and the Grenadines', 'VC', 'North America', 'XCD', '+1-784', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Samoa', 'WS', 'Oceania', 'WST', '+685', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'San Marino', 'SM', 'Europe', 'EUR', '+378', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Sao Tome and Principe', 'ST', 'Africa', 'STN', '+239', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Saudi Arabia', 'SA', 'Asia', 'SAR', '+966', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Senegal', 'SN', 'Africa', 'XOF', '+221', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Serbia', 'RS', 'Europe', 'RSD', '+381', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Seychelles', 'SC', 'Africa', 'SCR', '+248', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Sierra Leone', 'SL', 'Africa', 'SLL', '+232', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Singapore', 'SG', 'Asia', 'SGD', '+65', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Slovakia', 'SK', 'Europe', 'EUR', '+421', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Slovenia', 'SI', 'Europe', 'EUR', '+386', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Solomon Islands', 'SB', 'Oceania', 'SBD', '+677', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Somalia', 'SO', 'Africa', 'SOS', '+252', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'South Africa', 'ZA', 'Africa', 'ZAR', '+27', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'South Korea', 'KR', 'Asia', 'KRW', '+82', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'South Sudan', 'SS', 'Africa', 'SSP', '+211', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Spain', 'ES', 'Europe', 'EUR', '+34', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Sri Lanka', 'LK', 'Asia', 'LKR', '+94', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Sudan', 'SD', 'Africa', 'SDG', '+249', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Suriname', 'SR', 'South America', 'SRD', '+597', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Sweden', 'SE', 'Europe', 'SEK', '+46', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Switzerland', 'CH', 'Europe', 'CHF', '+41', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Syria', 'SY', 'Asia', 'SYP', '+963', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Taiwan', 'TW', 'Asia', 'TWD', '+886', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Tajikistan', 'TJ', 'Asia', 'TJS', '+992', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Tanzania', 'TZ', 'Africa', 'TZS', '+255', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Thailand', 'TH', 'Asia', 'THB', '+66', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Timor-Leste', 'TL', 'Asia', 'USD', '+670', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Togo', 'TG', 'Africa', 'XOF', '+228', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Tonga', 'TO', 'Oceania', 'TOP', '+676', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Trinidad and Tobago', 'TT', 'North America', 'TTD', '+1-868', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Tunisia', 'TN', 'Africa', 'TND', '+216', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Turkey', 'TR', 'Asia', 'TRY', '+90', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Turkmenistan', 'TM', 'Asia', 'TMT', '+993', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Tuvalu', 'TV', 'Oceania', 'AUD', '+688', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Uganda', 'UG', 'Africa', 'UGX', '+256', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Ukraine', 'UA', 'Europe', 'UAH', '+380', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'United Arab Emirates', 'AE', 'Asia', 'AED', '+971', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'United Kingdom', 'GB', 'Europe', 'GBP', '+44', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'United States', 'US', 'North America', 'USD', '+1', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Uruguay', 'UY', 'South America', 'UYU', '+598', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Uzbekistan', 'UZ', 'Asia', 'UZS', '+998', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Vanuatu', 'VU', 'Oceania', 'VUV', '+678', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Vatican City', 'VA', 'Europe', 'EUR', '+379', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Venezuela', 'VE', 'South America', 'VES', '+58', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Vietnam', 'VN', 'Asia', 'VND', '+84', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Yemen', 'YE', 'Asia', 'YER', '+967', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Zambia', 'ZM', 'Africa', 'ZMW', '+260', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL),
( 'Zimbabwe', 'ZW', 'Africa', 'ZWL', '+263', 1, 'System', '2024-01-01', NULL, NULL, 0, NULL, NULL);
    
    DECLARE @RowCount INT = (SELECT COUNT(*) FROM dbo.Country);
    PRINT '✅ Country data inserted successfully! Total: ' + CAST(@RowCount AS VARCHAR);
END
GO


