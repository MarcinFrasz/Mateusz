using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DbManipulationApp.Models
{
    public partial class czytaniaContext : DbContext
    {
        public czytaniaContext()
        {
        }

        public czytaniaContext(DbContextOptions<czytaniaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CzytaniaOld> CzytaniaOlds { get; set; } = null!;
        public virtual DbSet<FilmyOld> FilmyOlds { get; set; } = null!;
        public virtual DbSet<Kalendarz> Kalendarzs { get; set; } = null!;
        public virtual DbSet<Komentarze> Komentarzes { get; set; } = null!;
        public virtual DbSet<Ksiazki> Ksiazkis { get; set; } = null!;
        public virtual DbSet<Lekcjonarz> Lekcjonarzs { get; set; } = null!;
        public virtual DbSet<Patroni> Patronis { get; set; } = null!;
        public virtual DbSet<SKomentarzeZrodla> SKomentarzeZrodlas { get; set; } = null!;
        public virtual DbSet<STypCzytanium> STypCzytania { get; set; } = null!;
        public virtual DbSet<SlownikDni> SlownikDnis { get; set; } = null!;
        public virtual DbSet<Video> Videos { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CzytaniaOld>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("czytania_(old)");

                entity.Property(e => e.Data)
                    .HasColumnType("date")
                    .HasColumnName("data");

                entity.Property(e => e.Dzien)
                    .HasMaxLength(10)
                    .HasColumnName("dzien")
                    .IsFixedLength();

                entity.Property(e => e.Kmtauthor)
                    .HasMaxLength(10)
                    .HasColumnName("kmtauthor")
                    .IsFixedLength();

                entity.Property(e => e.Kmtcontent)
                    .HasMaxLength(10)
                    .HasColumnName("kmtcontent")
                    .IsFixedLength();

                entity.Property(e => e.Kmtfotolink)
                    .HasMaxLength(10)
                    .HasColumnName("kmtfotolink")
                    .IsFixedLength();

                entity.Property(e => e.Kmtlink)
                    .HasMaxLength(10)
                    .HasColumnName("kmtlink")
                    .IsFixedLength();

                entity.Property(e => e.Kmttitle)
                    .HasMaxLength(10)
                    .HasColumnName("kmttitle")
                    .IsFixedLength();

                entity.Property(e => e.Rozwazania)
                    .HasMaxLength(10)
                    .HasColumnName("rozwazania")
                    .IsFixedLength();

                entity.Property(e => e.Sigl)
                    .HasMaxLength(10)
                    .HasColumnName("sigl")
                    .IsFixedLength();

                entity.Property(e => e.Tekst)
                    .HasMaxLength(10)
                    .HasColumnName("tekst")
                    .IsFixedLength();

                entity.Property(e => e.Typ)
                    .HasMaxLength(10)
                    .HasColumnName("typ")
                    .IsFixedLength();

                entity.Property(e => e.Uwagi)
                    .HasMaxLength(10)
                    .HasColumnName("uwagi")
                    .IsFixedLength();
            });

            modelBuilder.Entity<FilmyOld>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("filmy_(old)");

                entity.Property(e => e.Data)
                    .HasColumnType("date")
                    .HasColumnName("data");

                entity.Property(e => e.Link1)
                    .HasMaxLength(10)
                    .HasColumnName("link1")
                    .IsFixedLength();

                entity.Property(e => e.Link2)
                    .HasMaxLength(10)
                    .HasColumnName("link2")
                    .IsFixedLength();

                entity.Property(e => e.Typ)
                    .HasMaxLength(10)
                    .HasColumnName("typ")
                    .IsFixedLength();
            });

            modelBuilder.Entity<Kalendarz>(entity =>
            {
                entity.HasKey(e => e.Data);

                entity.ToTable("Kalendarz");

                entity.Property(e => e.Data).HasPrecision(0);

                entity.Property(e => e.DzienLiturgiczny)
                    .HasMaxLength(8)
                    .HasColumnName("dzien_liturgiczny");

                entity.Property(e => e.IdKsiazka1).HasColumnName("id_ksiazka1");

                entity.Property(e => e.IdKsiazka2).HasColumnName("id_ksiazka2");

                entity.Property(e => e.IdKsiazka3).HasColumnName("id_ksiazka3");

                entity.Property(e => e.KomZrodloD)
                    .HasMaxLength(255)
                    .HasColumnName("kom_zrodloD");

                entity.Property(e => e.KomZrodloM)
                    .HasMaxLength(255)
                    .HasColumnName("kom_zrodloM");

                entity.Property(e => e.NazwaDnia)
                    .HasMaxLength(255)
                    .HasColumnName("nazwa_dnia");

                entity.Property(e => e.NumerTygodnia).HasColumnName("numer_tygodnia");

                entity.Property(e => e.Patron1)
                    .HasMaxLength(255)
                    .HasColumnName("patron1");

                entity.Property(e => e.Patron2)
                    .HasMaxLength(255)
                    .HasColumnName("patron2");

                entity.Property(e => e.Patron3)
                    .HasMaxLength(255)
                    .HasColumnName("patron3");

                entity.Property(e => e.RowVersion)
                    .HasColumnType("datetime")
                    .HasColumnName("row_version")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Komentarze>(entity =>
            {
                entity.HasKey(e => e.Idkom);

                entity.ToTable("Komentarze");

                entity.Property(e => e.Idkom).HasColumnName("idkom");

                entity.Property(e => e.DzienLiturgiczny)
                    .HasMaxLength(8)
                    .HasColumnName("dzien_liturgiczny");

                entity.Property(e => e.KomZrodlo)
                    .HasMaxLength(255)
                    .HasColumnName("kom_zrodlo");

                entity.Property(e => e.RowVersion)
                    .HasColumnType("datetime")
                    .HasColumnName("row_version")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Tekst).HasColumnName("tekst");
            });

            modelBuilder.Entity<Ksiazki>(entity =>
            {
                entity.HasKey(e => e.IdKsiazki);

                entity.ToTable("Ksiazki");

                entity.Property(e => e.IdKsiazki).HasColumnName("id_ksiazki");

                entity.Property(e => e.Autor).HasColumnName("autor");

                entity.Property(e => e.Foto).HasColumnName("foto");

                entity.Property(e => e.IdKmt)
                    .HasMaxLength(255)
                    .HasColumnName("id_kmt");

                entity.Property(e => e.Opis).HasColumnName("opis");

                entity.Property(e => e.RowVersion)
                    .HasColumnType("datetime")
                    .HasColumnName("row_version")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Tytul).HasColumnName("tytul");
            });

            modelBuilder.Entity<Lekcjonarz>(entity =>
            {
                entity.HasKey(e => e.IdLlekcjonarz);

                entity.ToTable("Lekcjonarz");

                entity.Property(e => e.IdLlekcjonarz).HasColumnName("id_llekcjonarz");

                entity.Property(e => e.DzienLiturgiczny)
                    .HasMaxLength(8)
                    .HasColumnName("dzien_liturgiczny");

                entity.Property(e => e.RowVersion)
                    .HasColumnType("datetime")
                    .HasColumnName("row_version")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Siglum)
                    .HasMaxLength(255)
                    .HasColumnName("siglum");

                entity.Property(e => e.Tekst).HasColumnName("tekst");

                entity.Property(e => e.TypCzytania)
                    .HasMaxLength(255)
                    .HasColumnName("typ_czytania");
            });

            modelBuilder.Entity<Patroni>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("patroni");

                entity.Property(e => e.Data)
                    .HasMaxLength(5)
                    .HasColumnName("data");

                entity.Property(e => e.Glowny).HasColumnName("glowny");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Opis).HasColumnName("opis");

                entity.Property(e => e.Patron)
                    .HasMaxLength(50)
                    .HasColumnName("patron");

                entity.Property(e => e.RowVersion)
                    .HasColumnType("datetime")
                    .HasColumnName("row_version")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Tekst)
                    .HasMaxLength(50)
                    .HasColumnName("tekst");

                entity.Property(e => e.Wyswietl).HasColumnName("wyswietl");
            });

            modelBuilder.Entity<SKomentarzeZrodla>(entity =>
            {
                entity.HasKey(e => e.SZrodla)
                    .HasName("PK_komentarze_zrodla");

                entity.ToTable("S_komentarze_zrodla");

                entity.Property(e => e.SZrodla)
                    .HasMaxLength(20)
                    .HasColumnName("s_zrodla")
                    .IsFixedLength();

                entity.Property(e => e.NazwaZrodla)
                    .HasMaxLength(100)
                    .HasColumnName("nazwa_zrodla")
                    .IsFixedLength();
            });

            modelBuilder.Entity<STypCzytanium>(entity =>
            {
                entity.HasKey(e => e.STypCzytania)
                    .HasName("PK_typ_czytania");

                entity.ToTable("S_typ_czytania");

                entity.Property(e => e.STypCzytania)
                    .HasMaxLength(2)
                    .HasColumnName("s_typ_czytania")
                    .IsFixedLength();
            });

            modelBuilder.Entity<SlownikDni>(entity =>
            {
                entity.HasKey(e => e.DzienLiturgiczny);

                entity.ToTable("SlownikDni");

                entity.Property(e => e.DzienLiturgiczny)
                    .HasMaxLength(255)
                    .HasColumnName("dzien_liturgiczny");

                entity.Property(e => e.NazwaDnia)
                    .HasMaxLength(255)
                    .HasColumnName("nazwa_dnia");

                entity.Property(e => e.RowVersion)
                    .HasColumnType("datetime")
                    .HasColumnName("row_version")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Swieto)
                    .HasColumnName("swieto")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Timestamp)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp");
            });

            modelBuilder.Entity<Video>(entity =>
            {
                entity.HasKey(e => e.IdVideo);

                entity.ToTable("Video");

                entity.Property(e => e.IdVideo).HasColumnName("id_Video");

                entity.Property(e => e.Data).HasPrecision(0);

                entity.Property(e => e.RowVersion)
                    .HasColumnType("datetime")
                    .HasColumnName("row_version")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TypCzytania)
                    .HasMaxLength(255)
                    .HasColumnName("typ_czytania");

                entity.Property(e => e.YoutubeId)
                    .HasMaxLength(255)
                    .HasColumnName("youtube_Id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
