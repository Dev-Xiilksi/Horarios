using Microsoft.EntityFrameworkCore;
using SistemaHorario.Domain.Entities;

namespace SistemaHorario.Infrastructure.Persistence;

public partial class AcademiaContext : DbContext
{
    public AcademiaContext()
    {
    }

    public AcademiaContext(DbContextOptions<AcademiaContext> options)
        : base(options)
    {
    }
    public virtual DbSet<AccesoUsuario> AccesoUsuarios { get; set; }
    public virtual DbSet<Area> Areas { get; set; }
    public virtual DbSet<Colegio> Colegios { get; set; }

    public virtual DbSet<HrGrado> HrGrados { get; set; }
    public virtual DbSet<Contratos1> Contratos1s { get; set; }

    public virtual DbSet<Dia> Dias { get; set; }

    public virtual DbSet<Emplegen> Emplegens { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<Grado> Grados { get; set; }

    public virtual DbSet<HrActividades> HrActividades { get; set; }

    public virtual DbSet<HrEvento> HrEventos { get; set; }

    public virtual DbSet<HrFrecuencia> HrFrecuencia { get; set; }

    public virtual DbSet<HrGeneral> HrGenerals { get; set; }

    public virtual DbSet<HrGeneral2> HrGeneral2s { get; set; }

    public virtual DbSet<HrHorario1> HrHorario1s { get; set; }

    public virtual DbSet<HrHorario2> HrHorario2s { get; set; }
    public virtual DbSet<HrLogErrors> HrLogErrors { get; set; }

    public virtual DbSet<HrHorarioExtra> HrHorarioExtras { get; set; }
    public virtual DbSet<HrVariable> HrVariable { get; set; }

    public virtual DbSet<Maestrosporseccion> Maestrosporseccions { get; set; }

    public virtual DbSet<Materia> Materias { get; set; }

    public virtual DbSet<Materiasporgrado> Materiasporgrados { get; set; }

    public virtual DbSet<Niveles> Niveles { get; set; }
    public virtual DbSet<Maestro> Maestros { get; set; }
    public virtual DbSet<CatalogoColegio> CatalogoColegios { get; set; }
    public virtual DbSet<Rol> Rols { get; set; }
    public virtual DbSet<HrBitacoraConfirmacion> HrBitacoraConfirmacion { get; set; }
    public virtual DbSet<HrTipoFranja> HrTipoFranja { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){ }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HrGrado>(entity =>
        {
            entity.ToTable("hr_grados");
            entity.Property(e => e.Colegio).IsFixedLength();
            entity.Property(e => e.Grado).IsFixedLength();
            entity.Property(e => e.Nivel).IsFixedLength();
            entity.Property(e => e.Seccion).IsFixedLength();
            entity.Property(e => e.NivelGrado);

            entity.HasOne(d => d.IdHorario1Navigation).WithMany(p => p.HrGrados)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_hr_grados_hr_horario1");
        });
        modelBuilder.Entity<HrTipoFranja>(entity =>
        {
            entity.HasKey(e => e.IdTipoFranja);
            entity.ToTable("hr_tipo_franja");
            entity.Property(e => e.IdTipoFranja).HasColumnName("idTipoFranja");
            entity.Property(e => e.Nombre)
                .IsUnicode(false)
                .HasColumnName("Nombre");
        });
        modelBuilder.Entity<AccesoUsuario>(entity =>
        {
            entity.HasKey(e => e.IdAcceso).HasName("PK__AccesoUs__34C1E8D79399DC2C");

            entity.ToTable("AccesoUsuario");

            entity.Property(e => e.IdAcceso).HasColumnName("idAcceso");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.IdArea).HasColumnName("idArea");
            entity.Property(e => e.IdEmpleado).HasColumnName("idEmpleado");
            entity.Property(e => e.IdRol).HasColumnName("idRol");

            entity.HasOne(d => d.IdAreaNavigation).WithMany(p => p.AccesoUsuarios)
                .HasForeignKey(d => d.IdArea)
                .HasConstraintName("FK_AccesoUsuario_Area");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.AccesoUsuarios)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__AccesoUsu__idRol__59662CFA");
        });
        modelBuilder.Entity<CatalogoColegio>(entity =>
        {
            entity.ToTable("CatalogoColegio");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Colegio)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("colegio");
            entity.Property(e => e.Grado)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("grado");
            entity.Property(e => e.Nivel)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nivel");
            entity.Property(e => e.Nombre)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Nomgrado)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("NOMGRADO");
            entity.Property(e => e.Nomnivel)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("NOMNIVEL");
            entity.Property(e => e.Notacolegio).HasColumnName("NOTACOLEGIO");
            entity.Property(e => e.Notaoficial).HasColumnName("NOTAOFICIAL");
            entity.Property(e => e.Numrecupera).HasColumnName("NUMRECUPERA");
            entity.Property(e => e.Seccion)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("seccion");
            entity.Property(e => e.Unidad)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("unidad");
            entity.Property(e => e.Unidades).HasColumnName("UNIDADES");
        });
        modelBuilder.Entity<Maestro>(entity =>
        {
            entity.HasKey(e => e.Codigo);

            entity.ToTable("MAESTROS");

            entity.Property(e => e.Codigo)
                .ValueGeneratedNever()
                .HasColumnName("CODIGO");
            entity.Property(e => e.Apell1)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("APELL1");
            entity.Property(e => e.Apell2)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("APELL2");
            entity.Property(e => e.Area).HasColumnName("AREA");
            entity.Property(e => e.Colegio)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("COLEGIO");
            entity.Property(e => e.Correo)
                .HasMaxLength(75)
                .HasColumnName("correo");
            entity.Property(e => e.Empleado).HasColumnName("empleado");
            entity.Property(e => e.MdluserId).HasColumnName("mdluser_id");
            entity.Property(e => e.Nom1)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("NOM1");
            entity.Property(e => e.Nom2)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("NOM2");
            entity.Property(e => e.Tipo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("tipo");
            entity.Property(e => e.Tipomaestro)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("tipomaestro");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");
        });
        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.Area1).HasFillFactor(90);

            entity.ToTable("areas");

            entity.HasIndex(e => e.Area1, "IX_areas").IsUnique();

            entity.Property(e => e.Area1)
                .HasDefaultValueSql("('')")
                .HasComment("AREAS ACADEMICAS A QUE CORRESPONDEN LAS MATERIAS QUE SE IMPARTEN")
                .HasColumnName("area");
            entity.Property(e => e.Codigo)
                .HasMaxLength(4)
                .IsFixedLength()
                .HasColumnName("codigo");
            entity.Property(e => e.MdlcategoryId).HasColumnName("mdlcategory_id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("nombre");
        });
        modelBuilder.Entity<HrLogErrors>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("hr_log_errors");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Mensaje)
                .IsUnicode(false)
                .HasColumnName("Mensaje");
            entity.Property(e => e.StackTrace)
                .IsUnicode(false)
                .HasColumnName("StackTrace");
            entity.Property(e => e.Fuente)
                .IsUnicode(false)
                .HasColumnName("Fuente");
            entity.Property(e => e.Metodo)
                .IsUnicode(false)
                .HasColumnName("Metodo");
            entity.Property(e => e.Usuario)
                .IsUnicode(false)
                .HasColumnName("Usuario");
            entity.Property(e => e.Fecha)
                .HasColumnName("Fecha");
            entity.Property(e => e.Hora)
                .HasColumnName("Hora");
        });
        modelBuilder.Entity<Colegio>(entity =>
        {
            entity.HasKey(e => e.Colegio1);

            entity.ToTable("COLEGIOS");

            entity.Property(e => e.Colegio1)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("COLEGIO");
            entity.Property(e => e.Empresa).HasColumnName("EMPRESA");
            entity.Property(e => e.Nombre)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
            entity.Property(e => e.Notacolegio).HasColumnName("NOTACOLEGIO");
            entity.Property(e => e.Notaoficial).HasColumnName("NOTAOFICIAL");
        });

        modelBuilder.Entity<Contratos1>(entity =>
        {
            entity.HasKey(e => new { e.Empresa, e.Empleado, e.Contrato });

            entity.ToTable("contratos1");

            entity.Property(e => e.Empresa).HasColumnName("empresa");
            entity.Property(e => e.Empleado).HasColumnName("empleado");
            entity.Property(e => e.Contrato).HasColumnName("contrato");
            entity.Property(e => e.Anticipo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("anticipo");
            entity.Property(e => e.Base).HasColumnName("base");
            entity.Property(e => e.Cuentaban)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("cuentaban");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.Fechae)
                .HasColumnType("datetime")
                .HasColumnName("fechae");
            entity.Property(e => e.Fechaf)
                .HasColumnType("datetime")
                .HasColumnName("fechaf");
            entity.Property(e => e.Fechai)
                .HasColumnType("datetime")
                .HasColumnName("fechai");
            entity.Property(e => e.Fpago).HasColumnName("fpago");
            entity.Property(e => e.Horafin)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("horafin");
            entity.Property(e => e.Horaini)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("horaini");
            entity.Property(e => e.Horas)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("horas");
            entity.Property(e => e.IdTemporalidad).HasColumnName("id_temporalidad");
            entity.Property(e => e.IdTipoContrato).HasColumnName("id_tipo_contrato");
            entity.Property(e => e.Jornada).HasColumnName("jornada");
            entity.Property(e => e.Mpago)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("mpago");
            entity.Property(e => e.Observa)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("observa");
            entity.Property(e => e.Puesto).HasColumnName("puesto");
            entity.Property(e => e.Semanahoras)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("semanahoras");
            entity.Property(e => e.Tipoempleado).HasColumnName("tipoempleado");
            entity.Property(e => e.Tipoper).HasColumnName("tipoper");
            entity.Property(e => e.Tiposeguro).HasColumnName("tiposeguro");

            entity.HasOne(d => d.Emplegen).WithMany(p => p.Contratos1s)
                .HasForeignKey(d => new { d.Empresa, d.Empleado })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_contratos1_emplegen");
        });

        modelBuilder.Entity<Dia>(entity =>
        {
            entity.HasKey(e => e.Dia1).HasName("PK__dias__D877D213762A9A13");

            entity.ToTable("dias");

            entity.Property(e => e.Dia1).HasColumnName("dia");
            entity.Property(e => e.Habil).HasColumnName("habil");
            entity.Property(e => e.Nombre)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Emplegen>(entity =>
        {
            entity.HasKey(e => new { e.Empresa, e.Empleado });
            entity.ToTable("emplegen");

            entity.Property(e => e.Empresa).HasColumnName("empresa");
            entity.Property(e => e.Empleado).HasColumnName("empleado");
            entity.Property(e => e.Apellido1)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("apellido1");
            entity.Property(e => e.Apellido2)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("apellido2");
            entity.Property(e => e.Apto)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("apto");
            entity.Property(e => e.Calle)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("calle");
            entity.Property(e => e.Casada)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("casada");
            entity.Property(e => e.Celular)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("celular");
            entity.Property(e => e.Civil)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("civil");
            entity.Property(e => e.Colonia)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("colonia");
            entity.Property(e => e.Correo)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.Correoi)
                .HasMaxLength(50)
                .HasColumnName("correoi");
            entity.Property(e => e.Deptoced).HasColumnName("deptoced");
            entity.Property(e => e.Deptodir).HasColumnName("deptodir");
            entity.Property(e => e.Direccion)
                .HasMaxLength(75)
                .IsUnicode(false)
                .HasColumnName("direccion");
            entity.Property(e => e.Docdocente)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("docdocente");
            entity.Property(e => e.Economica).HasColumnName("economica");
            entity.Property(e => e.Etnia)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("etnia");
            entity.Property(e => e.Fechai)
                .HasColumnType("datetime")
                .HasColumnName("fechai");
            entity.Property(e => e.Fechanac)
                .HasColumnType("datetime")
                .HasColumnName("fechanac");
            entity.Property(e => e.IdArea).HasColumnName("id_area");
            entity.Property(e => e.IdComunidadLing).HasColumnName("id_comunidad_ling");
            entity.Property(e => e.IdNivelEducativo).HasColumnName("id_nivel_educativo");
            entity.Property(e => e.IdPuebloPertenencia).HasColumnName("id_pueblo_pertenencia");
            entity.Property(e => e.IdTipoDiscapacidad).HasColumnName("id_tipo_discapacidad");
            entity.Property(e => e.Municed).HasColumnName("municed");
            entity.Property(e => e.Munidir).HasColumnName("munidir");
            entity.Property(e => e.Nacional).HasColumnName("nacional");
            entity.Property(e => e.Nit)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nit");
            entity.Property(e => e.Nombre1)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nombre1");
            entity.Property(e => e.Nombre2)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nombre2");
            entity.Property(e => e.Nombre3)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nombre3");
            entity.Property(e => e.Numero)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("numero");
            entity.Property(e => e.Numidentica)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("numidentica");
            entity.Property(e => e.Numseguro)
                .HasMaxLength(14)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("numseguro");
            entity.Property(e => e.Registro)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("registro");
            entity.Property(e => e.Sexo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("sexo");
            entity.Property(e => e.Telefono)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("telefono");
            entity.Property(e => e.Tipoiden).HasColumnName("tipoiden");
            entity.Property(e => e.TituloPrincipal)
                .HasMaxLength(150)
                .HasColumnName("titulo_principal");
            entity.Property(e => e.Usuario)
                .HasMaxLength(50)
                .HasColumnName("usuario");
            entity.Property(e => e.Zona).HasColumnName("zona");

            entity.HasOne(d => d.EmpresaNavigation).WithMany(p => p.Emplegens)
                .HasForeignKey(d => d.Empresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_emplegen_empresas");
        });

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.Empresa1).HasFillFactor(90);

            entity.ToTable("empresas");

            entity.Property(e => e.Empresa1)
                .ValueGeneratedNever()
                .HasColumnName("empresa");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.Ajuste).HasColumnName("ajuste");
            entity.Property(e => e.Alu).HasColumnName("alu");
            entity.Property(e => e.Boleta).HasColumnName("boleta");
            entity.Property(e => e.Caja).HasColumnName("caja");
            entity.Property(e => e.Contador)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("contador");
            entity.Property(e => e.Correo)
                .HasMaxLength(40)
                .HasColumnName("correo");
            entity.Property(e => e.Corte).HasColumnName("corte");
            entity.Property(e => e.Ctacobrar)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CTACOBRAR");
            entity.Property(e => e.Cuentaid)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("cuentaid");
            entity.Property(e => e.Cuentaih)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("cuentaih");
            entity.Property(e => e.Debito).HasColumnName("debito");
            entity.Property(e => e.Departamento).HasColumnName("departamento");
            entity.Property(e => e.Depo).HasColumnName("depo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("direccion");
            entity.Property(e => e.Exenta)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("EXENTA");
            entity.Property(e => e.Factura).HasColumnName("FACTURA");
            entity.Property(e => e.FacturaExenta).HasColumnName("factura_exenta");
            entity.Property(e => e.FacturaIsr).HasColumnName("factura_isr");
            entity.Property(e => e.Fam).HasColumnName("fam");
            entity.Property(e => e.Igss)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("igss");
            entity.Property(e => e.Ingreso).HasColumnName("INGRESO");
            entity.Property(e => e.Insbus).HasColumnName("insbus");
            entity.Property(e => e.Iva)
                .HasColumnType("decimal(4, 2)")
                .HasColumnName("IVA");
            entity.Property(e => e.Mesffiscal)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("mesffiscal");
            entity.Property(e => e.Mesifiscal)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("mesifiscal");
            entity.Property(e => e.Moneda)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("moneda");
            entity.Property(e => e.Municipio).HasColumnName("municipio");
            entity.Property(e => e.Nit)
                .HasMaxLength(17)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nit");
            entity.Property(e => e.Nombre)
                .HasMaxLength(45)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nombre");
            entity.Property(e => e.Nombre2)
                .HasMaxLength(15)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nombre2");
            entity.Property(e => e.Nota).HasColumnName("nota");
            entity.Property(e => e.Notacta).HasColumnName("notacta");
            entity.Property(e => e.Notad).HasColumnName("notad");
            entity.Property(e => e.Numcons).HasColumnName("numcons");
            entity.Property(e => e.Orden).HasColumnName("orden");
            entity.Property(e => e.PaginaWeb)
                .HasMaxLength(40)
                .HasColumnName("pagina_web");
            entity.Property(e => e.Pedido).HasColumnName("pedido");
            entity.Property(e => e.Poliza).HasColumnName("poliza");
            entity.Property(e => e.Proveedor).HasColumnName("proveedor");
            entity.Property(e => e.Provision).HasColumnName("provision");
            entity.Property(e => e.Recha).HasColumnName("recha");
            entity.Property(e => e.Recibo).HasColumnName("recibo");
            entity.Property(e => e.Redi).HasColumnName("redi");
            entity.Property(e => e.Representante)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("representante");
            entity.Property(e => e.Rescate).HasColumnName("rescate");
            entity.Property(e => e.Rete).HasColumnName("rete");
            entity.Property(e => e.Reti).HasColumnName("reti");
            entity.Property(e => e.Reticol).HasColumnName("reticol");
            entity.Property(e => e.Salida).HasColumnName("SALIDA");
            entity.Property(e => e.Serie)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SERIE");
            entity.Property(e => e.SerieExenta)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("serie_exenta");
            entity.Property(e => e.SerieIsr)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("serie_isr");
            entity.Property(e => e.Tarjeta).HasColumnName("tarjeta");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");
            entity.Property(e => e.Vale).HasColumnName("VALE");
        });

        modelBuilder.Entity<Grado>(entity =>
        {
            entity.HasKey(e => new { e.Colegio, e.Nivel, e.Grado1 });

            entity.ToTable("GRADOS");

            entity.Property(e => e.Colegio)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("COLEGIO");
            entity.Property(e => e.Nivel)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("NIVEL");
            entity.Property(e => e.Grado1)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("GRADO");
            entity.Property(e => e.Nombre)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
            entity.Property(e => e.Origen)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ORIGEN");
            entity.Property(e => e.Real).HasColumnName("real");
        });

        modelBuilder.Entity<HrActividades>(entity =>
        {
            entity.HasKey(e => e.IdHra).HasName("PK__hr_activ__D6EACDD8886C00DF");

            entity.ToTable("hr_actividades");

            entity.Property(e => e.IdHra).HasColumnName("id_hra");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.IdTipoOpcion).HasColumnName("id_tipo_opcion");
        });

        modelBuilder.Entity<HrEvento>(entity =>
        {
            entity.HasKey(e => e.IdHex);

            entity.ToTable("hr_eventos");

            entity.Property(e => e.IdHex).HasColumnName("id_hex");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<HrFrecuencia>(entity =>
        {
            entity.HasKey(e => e.IdHrf).HasName("PK__hr_frecu__D6EACDD53C7A0F5B");

            entity.ToTable("hr_frecuencia");

            entity.Property(e => e.IdHrf).HasColumnName("id_hrf");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<HrGeneral>(entity =>
        {
            entity.HasKey(e => e.IdHrg1).HasName("PK__hr_gener__BF69EB508851CCF1");

            entity.ToTable("hr_general");

            entity.Property(e => e.IdHrg1).HasColumnName("id_hrg1");
            entity.Property(e => e.Ciclo)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ciclo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<HrBitacoraConfirmacion>(entity =>
        {
            entity.HasKey(e => e.id_bitacora_confirmacion);

            entity.ToTable("hr_bitacora_confirmacion");

            entity.Property(e => e.id_bitacora_confirmacion).HasColumnName("id_bitacora_confirmacion");
            entity.Property(e => e.id_horario1)
                .HasColumnName("id_horario1");
            entity.Property(e => e.id_coordinador)
                .HasColumnName("id_coordinador");
            entity.Property(e => e.hora_registro)
                .HasColumnName("hora_registro");
            entity.Property(e => e.registro_fecha)
                .HasColumnName("registro_fecha");
        });

        modelBuilder.Entity<HrGeneral2>(entity =>
        {
            entity.HasKey(e => e.IdHrg2).HasName("PK__hr_gener__BF69EB51AC6666EA");

            entity.ToTable("hr_general2");

            entity.Property(e => e.IdHrg2).HasColumnName("id_hrg2");
            entity.Property(e => e.Horaf)
                .HasMaxLength(8)
                .IsFixedLength()
                .HasColumnName("horaf");
            entity.Property(e => e.Horai)
                .HasMaxLength(8)
                .IsFixedLength()
                .HasColumnName("horai");
            entity.Property(e => e.IdHrg1).HasColumnName("id_hrg1");
            entity.Property(e => e.Tiempo)
                .HasMaxLength(8)
                .IsFixedLength()
                .HasColumnName("tiempo");

            entity.HasOne(d => d.IdHrg1Navigation).WithMany(p => p.HrGeneral2s)
                .HasForeignKey(d => d.IdHrg1)
                .HasConstraintName("FK__hr_genera__id_hr__288DEB75");
        });

        modelBuilder.Entity<HrHorario1>(entity =>
        {
            entity.HasKey(e => e.IdHorario1);

            entity.ToTable("hr_horario1");

            entity.Property(e => e.IdHorario1).HasColumnName("id_horario1");
            entity.Property(e => e.Area).HasColumnName("area");
            entity.Property(e => e.Ciclo)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ciclo");
            entity.Property(e => e.Colegio)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("colegio");
            entity.Property(e => e.CorrecionTrabajos)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("correcion_trabajos");
            entity.Property(e => e.Empleado).HasColumnName("empleado");
            entity.Property(e => e.Empresa).HasColumnName("empresa");
            entity.Property(e => e.Estudiantes).HasColumnName("estudiantes");
            entity.Property(e => e.TipoFranja).HasColumnName("TipoFranja");
            entity.Property(e => e.Grado)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("grado");
            entity.Property(e => e.IdHrg1).HasColumnName("id_hrg1");
            entity.Property(e => e.Nivel)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nivel");
            entity.Property(e => e.Observacion)
                .HasMaxLength(3000)
                .IsUnicode(false)
                .HasColumnName("observacion");
            entity.Property(e => e.Seccion)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("seccion");
            entity.Property(e => e.Fecha_Registro)
                .HasColumnName("fecha_registro");
            entity.Property(e => e.Hora_Registro)
                .HasColumnName("hora_registro");

            entity.HasOne(d => d.IdHrg1Navigation).WithMany(p => p.HrHorario1s)
                .HasForeignKey(d => d.IdHrg1)
                .HasConstraintName("FK_hr_horario1_hr_horario1");

            entity.HasOne(d => d.Emplegen).WithMany(p => p.HrHorario1s)
                .HasForeignKey(d => new { d.Empresa, d.Empleado })
                .HasConstraintName("FK_hr_horario1_emplegen");
        });

        modelBuilder.Entity<HrHorario2>(entity =>
        {
            entity.HasKey(e => e.IdHorario2).HasName("PK__hr_horar__38BAA5E5EE671494");

            entity.ToTable("hr_horario2");

            entity.Property(e => e.IdHorario2).HasColumnName("id_horario2");
            entity.Property(e => e.Dia).HasColumnName("dia");
            entity.Property(e => e.Horaf)
                .HasMaxLength(5)
                .IsFixedLength()
                .HasColumnName("horaf");
            entity.Property(e => e.Horai)
                .HasMaxLength(5)
                .IsFixedLength()
                .HasColumnName("horai");
            entity.Property(e => e.IdHorario1).HasColumnName("id_horario1");
            entity.Property(e => e.IdHra).HasColumnName("id_hra");
            entity.Property(e => e.IdHrg2).HasColumnName("id_hrg2");
            entity.Property(e => e.IdMps).HasColumnName("id_mps");
            entity.Property(e => e.Tiempo)
                .HasMaxLength(5)
                .IsFixedLength()
                .HasColumnName("tiempo");
            entity.Property(e => e.NumeroFila)
                .HasColumnName("numero_fila");

            entity.HasOne(d => d.DiaNavigation).WithMany(p => p.HrHorario2s)
                .HasForeignKey(d => d.Dia)
                .HasConstraintName("FK__hr_horario2__dia__461E4E5C");

            entity.HasOne(d => d.IdHorario1Navigation).WithMany(p => p.HrHorario2s)
                .HasForeignKey(d => d.IdHorario1)
                .HasConstraintName("FK__hr_horari__id_ho__452A2A23");

            entity.HasOne(d => d.IdHraNavigation).WithMany(p => p.HrHorario2s)
                .HasForeignKey(d => d.IdHra)
                .HasConstraintName("FK__hr_horari__id_hr__48FABB07");

            entity.HasOne(d => d.IdHrg2Navigation).WithMany(p => p.HrHorario2s)
                .HasForeignKey(d => d.IdHrg2)
                .HasConstraintName("FK__hr_horari__id_hr__47127295");

            entity.HasOne(d => d.IdMpsNavigation).WithMany(p => p.HrHorario2s)
                .HasForeignKey(d => d.IdMps)
                .HasConstraintName("FK__hr_horari__id_mp__480696CE");
        });

        modelBuilder.Entity<HrHorarioExtra>(entity =>
        {
            entity.HasKey(e => e.IdHrEx).HasName("PK__hr_horar__D50B65124884201B");

            entity.ToTable("hr_horario_extra");

            entity.Property(e => e.IdHrEx).HasColumnName("id_hr_ex");
            entity.Property(e => e.IdHex).HasColumnName("id_hex");
            entity.Property(e => e.IdHorario1).HasColumnName("id_horario1");
            entity.Property(e => e.IdHrf).HasColumnName("id_hrf");

            entity.HasOne(d => d.IdHexNavigation).WithMany(p => p.HrHorarioExtras)
                .HasForeignKey(d => d.IdHex)
                .HasConstraintName("FK_hr_horario_extra_hr_eventos");

            entity.HasOne(d => d.IdHorario1Navigation).WithMany(p => p.HrHorarioExtras)
                .HasForeignKey(d => d.IdHorario1)
                .HasConstraintName("FK__hr_horari__id_ho__4BD727B2");

            entity.HasOne(d => d.IdHrfNavigation).WithMany(p => p.HrHorarioExtras)
                .HasForeignKey(d => d.IdHrf)
                .HasConstraintName("FK__hr_horari__id_hr__4DBF7024");
        });
        modelBuilder.Entity<HrVariable>(entity =>
        {
            entity.HasKey(e => e.IdHrVariable);

            entity.ToTable("hr_variable");

            entity.Property(e => e.IdHrVariable).HasColumnName("id_hr_variable");
            entity.Property(e => e.HorarioI)
                .HasMaxLength(8)
                .IsFixedLength()
                .HasColumnName("horarioi");
            entity.Property(e => e.HorarioF)
                .HasMaxLength(8)
                .IsFixedLength()
                .HasColumnName("horariof");
        });

        modelBuilder.Entity<Maestrosporseccion>(entity =>
        {
            entity.HasKey(e => e.IdMps);

            entity.ToTable("MAESTROSPORSECCION");

            entity.Property(e => e.IdMps).HasColumnName("id_mps");
            entity.Property(e => e.Ciclo)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ciclo");
            entity.Property(e => e.Codigo).HasColumnName("CODIGO");
            entity.Property(e => e.Codmateria).HasColumnName("CODMATERIA");
            entity.Property(e => e.Colegio)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("COLEGIO");
            entity.Property(e => e.Grado)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("GRADO");
            entity.Property(e => e.MdlgroupId).HasColumnName("mdlgroup_id");
            entity.Property(e => e.Nivel)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("NIVEL");
            entity.Property(e => e.Seccion)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SECCION");
        });

        modelBuilder.Entity<Materia>(entity =>
        {
            entity.HasKey(e => new { e.Colegio, e.Codigo });

            entity.ToTable("MATERIAS");

            entity.Property(e => e.Colegio)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("COLEGIO");
            entity.Property(e => e.Codigo).HasColumnName("CODIGO");
            entity.Property(e => e.Area).HasColumnName("AREA");
            entity.Property(e => e.Aspecto)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ASPECTO");
            entity.Property(e => e.Codpadre).HasColumnName("CODPADRE");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
            entity.Property(e => e.Nombrec)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("NOMBREC");
            entity.Property(e => e.Tipo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TIPO");
            entity.Property(e => e.Ver).HasColumnName("VER");
        });

        modelBuilder.Entity<Materiasporgrado>(entity =>
        {
            entity.HasKey(e => e.IdMg);

            entity.ToTable("MATERIASPORGRADO");

            entity.Property(e => e.IdMg).HasColumnName("id_mg");
            entity.Property(e => e.Ciclo)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CICLO");
            entity.Property(e => e.Codigo).HasColumnName("CODIGO");
            entity.Property(e => e.Colegio)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("COLEGIO");
            entity.Property(e => e.Grado)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("GRADO");
            entity.Property(e => e.MdlcourseId).HasColumnName("mdlcourse_id");
            entity.Property(e => e.Nivel)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("NIVEL");
            entity.Property(e => e.Orden).HasColumnName("ORDEN");
            entity.Property(e => e.Promedio)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("promedio");
            entity.Property(e => e.Visible).HasColumnName("visible");
        });
        
        modelBuilder.Entity<Niveles>(entity =>
        {
            entity.HasKey(e => new { e.Colegio, e.Nivel });

            entity.ToTable("NIVELES");

            entity.Property(e => e.Colegio)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("COLEGIO");
            entity.Property(e => e.Nivel)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("NIVEL");
            entity.Property(e => e.Nombre)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
            entity.Property(e => e.Orden).HasColumnName("ORDEN");
            entity.Property(e => e.Promo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("PROMO");
        });
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol);

            entity.ToTable("rol");

            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("nombre");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
