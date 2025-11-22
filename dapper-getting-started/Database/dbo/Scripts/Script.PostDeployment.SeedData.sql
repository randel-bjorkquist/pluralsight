/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

/***** SEED DATA FOR STATES TABLE *****/
INSERT [dbo].[State] ([ID], [Name], [Abbreviation]) 
VALUES ( 1 ,N'Alabama'              ,N'AL')
      ,( 2 ,N'Alaska'               ,N'AK')
      ,( 4 ,N'Arizona'              ,N'AZ')
      ,( 5 ,N'Arkansas'             ,N'AR')
      ,( 6 ,N'California'           ,N'CA')
      ,( 8 ,N'Colorado'             ,N'CO')
      ,( 9 ,N'Connecticut'          ,N'CT')
      ,(10 ,N'Delaware'             ,N'DE')
      ,(11 ,N'District of Columbia' ,N'DC')
      ,(12 ,N'Florida'              ,N'FL')
      ,(13 ,N'Georgia'              ,N'GA')
      ,(15 ,N'Hawaii'               ,N'HI')
      ,(16 ,N'Idaho'                ,N'ID')
      ,(17 ,N'Illinois'             ,N'IL')
      ,(18 ,N'Indiana'              ,N'IN')
      ,(19 ,N'Iowa'                 ,N'IA')
      ,(20 ,N'Kansas'               ,N'KS')
      ,(21 ,N'Kentucky'             ,N'KY')
      ,(22 ,N'Louisiana'            ,N'LA')
      ,(23 ,N'Maine'                ,N'ME')
      ,(24 ,N'Maryland'             ,N'MD')
      ,(25 ,N'Massachusetts'        ,N'MA')
      ,(26 ,N'Michigan'             ,N'MI')
      ,(27 ,N'Minnesota'            ,N'MN')
      ,(28 ,N'Mississippi'          ,N'MS')
      ,(29 ,N'Missouri'             ,N'MO')
      ,(30 ,N'Montana'              ,N'MT')
      ,(31 ,N'Nebraska'             ,N'NE')
      ,(32 ,N'Nevada'               ,N'NV')
      ,(33 ,N'New Hampshire'        ,N'NH')
      ,(34 ,N'New Jersey'           ,N'NJ')
      ,(35 ,N'New Mexico'           ,N'NM')
      ,(36 ,N'New York'             ,N'NY')
      ,(37 ,N'North Carolina'       ,N'NC')
      ,(38 ,N'North Dakota'         ,N'ND')
      ,(39 ,N'Ohio'                 ,N'OH')
      ,(40 ,N'Oklahoma'             ,N'OK')
      ,(41 ,N'Oregon'               ,N'OR')
      ,(42 ,N'Pennsylvania'         ,N'PA')
      ,(44 ,N'Rhode Island'         ,N'RI')
      ,(45 ,N'South Carolina'       ,N'SC')
      ,(46 ,N'South Dakota'         ,N'SD')
      ,(47 ,N'Tennessee'            ,N'TN')
      ,(48 ,N'Texas'                ,N'TX')
      ,(49 ,N'Utah'                 ,N'UT')
      ,(50 ,N'Vermont'              ,N'VT')
      ,(51 ,N'Virginia'             ,N'VA')
      ,(53 ,N'Washington'           ,N'WA')
      ,(54 ,N'West Virginia'        ,N'WV')
      ,(55 ,N'Wisconsin'            ,N'WI')
      ,(56 ,N'Wyoming'              ,N'WY');
GO

/***** SEED DATA FOR CONTACTS TABLE *****/
INSERT [Contact]
VALUES ('Michael', 'Jordan', 'michael@bulls.com', 'Chicago Bulls', 'MVP')
      ,('LaBron', 'James', 'labron@lakers.com', 'Los Angeles Lakers', 'King James')
      ,('Giannis', 'Antetokounmpo', 'giannis@bucks.com', 'Milwaukee Bucks', 'The Greek Freak')
      ,('Kevin', 'Durant', 'kevin@warriors.com', 'Golden State Warriors', 'KD')
      ,('Kyrie', 'Irving', 'kyrie@celtics.com', 'Boston Celtics', 'Uncle Drew')
      ,('James', 'Harden', 'james@rockets.com', 'Houston Rockets', 'The Beard');
GO

/***** SEED DATA FOR ADDRESSES TABLE *****/
INSERT [Address] 
VALUES (1, 'Home', '123 Main Street', 'Chicago', 17, '60290')
      ,(1, 'Work', '1901 W Madison St', 'Chicago', 17, '60612')
      ,(2, 'Home', '123 Main Street', 'Los Angeles', 6, '90001')
      ,(3, 'Home', '123 Main Street', 'Milwaukee', 55, '53201')
      ,(4, 'Home', '123 Main Street', 'Oakland', 6, '94577')
      ,(5, 'Home', '123 Main Street', 'Boston', 25, '02101')
      ,(6, 'Home', '456 Main Street', 'Houston', 48, '77001');
GO
