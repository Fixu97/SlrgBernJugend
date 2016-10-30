USE SLRG_BERN_JUGEND;

GO

CREATE PROCEDURE InsertPerson(
@Prename nvarchar(32),
@Lastname nvarchar(32),
@Birthday date,
@Male bit,
@Active bit,
@PhoneNr nvarchar(12),
@Email nvarchar(32),
@UserPk int
)
AS
BEGIN

	INSERT INTO PEOPLE
	(Prename,Lastname,Birthday,Male,Active,PhoneNr,Email) VALUES
	(@Prename,@Lastname,@Birthday,@Male,@Active,@PhoneNr,@Email);
	
	DECLARE @PersonPk int = (SELECT TOP(1) PK FROM PEOPLE ORDER BY PK DESC);
	DECLARE @AdminPk int;

	DECLARE ADMIN_CURSOR CURSOR FOR
		(SELECT PK FROM USERS WHERE [Admin] = 1 OR PK = @UserPk);
		OPEN ADMIN_CURSOR;
			FETCH NEXT FROM ADMIN_CURSOR INTO @AdminPk;
			WHILE @@FETCH_STATUS = 0
			BEGIN
				INSERT INTO PERMISSIONS
				(FK_U,FK_P)VALUES
				(@AdminPk,@PersonPk);
				FETCH NEXT FROM ADMIN_CURSOR INTO @AdminPk
			END
		CLOSE ADMIN_CURSOR
	DEALLOCATE ADMIN_CURSOR;
END