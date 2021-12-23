Create database GestioneSpese

create table Categorie (
Id int not null constraint PK_IdCategoria primary key identity(1, 1),
Categoria varchar(100)
)

create table Spese (
Id int not null constraint PK_IdSpesa primary key identity(1, 1),
DataSpesa datetime,
CategoriaId int not null foreign key references Categorie(Id),
Descrizione varchar(500),
Utente varchar(100),
Importo decimal(4, 2),
Approvato bit
)

-- procedure inserimento
create procedure InserisciCategoria
@nomeCategoria varchar(100)
as
insert into Categorie values (@nomeCategoria)
go

create procedure InserisciSpesa
@data datetime,
@nomeCategoria varchar(100),
@descriz varchar(500),
@utente varchar(100),
@importo decimal(4, 2)
as
declare @idCat int
select @idCat = c.Id from Categorie c where c.Categoria = @nomeCategoria
insert into Spese values (@data, @idCat, @descriz, @utente, @importo, 0)
go

-- inserimento categorie
execute InserisciCategoria 'Trasporti'
execute InserisciCategoria 'Spese mediche'
execute InserisciCategoria 'Istruzione'
execute InserisciCategoria 'Svago'
execute InserisciCategoria 'Alimentari'

-- inserimento spese
execute InserisciSpesa '2021/02/15', 'Trasporti', 'spesa 1', 'Gino Paoli', 20
execute InserisciSpesa '2021/02/16', 'Alimentari', 'spesa 8', 'Gino Paoli', 40
execute InserisciSpesa '2021/02/18', 'Spese mediche', 'spesa 2', 'Massimo Medda', 30
execute InserisciSpesa '2021/05/09', 'Istruzione', 'spesa 3', 'Lina Werthmuller', 50
execute InserisciSpesa '2021/06/20', 'Svago', 'spesa 4', 'Anna Mazzamauro', 33
execute InserisciSpesa '2021/08/18', 'Alimentari', 'spesa 5', 'Renato Pozzetto', 40
execute InserisciSpesa '2021/10/01', 'Spese mediche', 'spesa 6', 'Sora Lella', 30
execute InserisciSpesa '2021/02/15', 'Trasporti', 'spesa 7', 'Mario brega', 78

select * from spese

-- groupby
select c.Categoria, joinByCat.TotImporto
from Categorie c join (
select s.CategoriaId, sum(s.Importo) as TotImporto
from spese s
group by s.CategoriaId) joinByCat
on c.Id = joinByCat.CategoriaId
