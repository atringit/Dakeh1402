using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dake.Migrations
{
    public partial class initialCatalog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AboutUss",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    description = table.Column<string>(maxLength: 5000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutUss", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Categorys",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    registerPrice = table.Column<long>(nullable: false),
                    expirePrice = table.Column<long>(nullable: false),
                    espacialPrice = table.Column<long>(nullable: false),
                    parentCategoryId = table.Column<int>(nullable: true),
                    name = table.Column<string>(maxLength: 200, nullable: false),
                    image = table.Column<string>(maxLength: 500, nullable: true),
                    laderPrice = table.Column<long>(nullable: false),
                    emergencyPrice = table.Column<long>(nullable: false),
                    staticemergencyPriceId = table.Column<string>(nullable: true),
                    staticregisterPriceId = table.Column<string>(nullable: true),
                    staticexpirePriceId = table.Column<string>(nullable: true),
                    staticespacialPriceId = table.Column<string>(nullable: true),
                    staticladerPriceId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorys", x => x.id);
                    table.ForeignKey(
                        name: "FK_Categorys_Categorys_parentCategoryId",
                        column: x => x.parentCategoryId,
                        principalTable: "Categorys",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ContactUss",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    phone = table.Column<string>(maxLength: 50, nullable: true),
                    pageTelegramUrl = table.Column<string>(maxLength: 200, nullable: true),
                    pageInstagramUrl = table.Column<string>(maxLength: 200, nullable: true),
                    pageTwitterUrl = table.Column<string>(maxLength: 200, nullable: true),
                    email = table.Column<string>(maxLength: 200, nullable: true),
                    androidVersion = table.Column<string>(maxLength: 20, nullable: true),
                    PageEitta = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactUss", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "DiscountCodes",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    code = table.Column<int>(nullable: false),
                    count = table.Column<int>(nullable: false),
                    remain = table.Column<int>(nullable: false),
                    price = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountCodes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Informations",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    title = table.Column<string>(maxLength: 500, nullable: false),
                    description = table.Column<string>(maxLength: 5000, nullable: false),
                    Link = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Informations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentRequestAttemps",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    NoticeId = table.Column<long>(nullable: false),
                    FactorId = table.Column<int>(nullable: false),
                    pursheType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRequestAttemps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleNameFa = table.Column<string>(maxLength: 1000, nullable: false),
                    RoleNameEn = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rules",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    description = table.Column<string>(maxLength: 5000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rules", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    wrongWord = table.Column<string>(maxLength: 2000, nullable: false),
                    countExpireDate = table.Column<int>(nullable: true),
                    countExpireDateIsespacial = table.Column<int>(nullable: true),
                    countExpireDateEmergency = table.Column<int>(nullable: true),
                    showPriceForCars = table.Column<bool>(nullable: false),
                    AutoAccept = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Sliders",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    image = table.Column<string>(maxLength: 500, nullable: true),
                    link = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sliders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "StaticPrices",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    price = table.Column<long>(nullable: false),
                    code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaticPrices", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Stirs",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    description = table.Column<string>(maxLength: 5000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stirs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: true),
                    cityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.id);
                    table.ForeignKey(
                        name: "FK_Provinces_Cities_cityId",
                        column: x => x.cityId,
                        principalTable: "Cities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InformationMedias",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Image = table.Column<string>(nullable: true),
                    InformationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InformationMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InformationMedias_Informations_InformationId",
                        column: x => x.InformationId,
                        principalTable: "Informations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: true),
                    provinceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.id);
                    table.ForeignKey(
                        name: "FK_Areas_Provinces_provinceId",
                        column: x => x.provinceId,
                        principalTable: "Provinces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    cellphone = table.Column<string>(maxLength: 11, nullable: false),
                    password = table.Column<string>(maxLength: 200, nullable: true),
                    passwordShow = table.Column<string>(maxLength: 20, nullable: true),
                    token = table.Column<string>(maxLength: 100, nullable: true),
                    roleId = table.Column<int>(nullable: false),
                    code = table.Column<string>(maxLength: 6, nullable: true),
                    isCodeConfirmed = table.Column<bool>(nullable: false),
                    oTPDate = table.Column<DateTime>(maxLength: 50, nullable: false),
                    provinceId = table.Column<int>(nullable: true),
                    adminRole = table.Column<string>(maxLength: 50, nullable: true),
                    IsBlocked = table.Column<bool>(nullable: false),
                    PushNotifToken = table.Column<string>(nullable: true),
                    deleted = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                    table.ForeignKey(
                        name: "FK_Users_Provinces_provinceId",
                        column: x => x.provinceId,
                        principalTable: "Provinces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Roles_roleId",
                        column: x => x.roleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdminsInCities",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    userid = table.Column<int>(nullable: false),
                    cityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminsInCities", x => x.id);
                    table.ForeignKey(
                        name: "FK_AdminsInCities_Cities_cityId",
                        column: x => x.cityId,
                        principalTable: "Cities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdminsInCities_Users_userid",
                        column: x => x.userid,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Banner",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    title = table.Column<string>(nullable: true),
                    notConfirmDescription = table.Column<string>(maxLength: 1000, nullable: true),
                    Link = table.Column<string>(maxLength: 1000, nullable: true),
                    adminConfirmStatus = table.Column<int>(nullable: false),
                    userId = table.Column<int>(nullable: false),
                    code = table.Column<string>(nullable: true),
                    createDate = table.Column<DateTime>(nullable: false),
                    expireDate = table.Column<DateTime>(nullable: false),
                    expireDateIsespacial = table.Column<DateTime>(nullable: true),
                    countView = table.Column<int>(nullable: false),
                    isSpecial = table.Column<bool>(nullable: false),
                    isEmergency = table.Column<bool>(nullable: false),
                    ExpireDateEmergency = table.Column<DateTime>(nullable: true),
                    isPaid = table.Column<bool>(nullable: false),
                    AdminUserAccepted = table.Column<string>(nullable: true),
                    AcceptedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banner", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Banner_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notices",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    movie = table.Column<string>(maxLength: 500, nullable: true),
                    image = table.Column<string>(maxLength: 500, nullable: true),
                    title = table.Column<string>(maxLength: 200, nullable: false),
                    price = table.Column<long>(nullable: false),
                    lastPrice = table.Column<long>(nullable: false),
                    description = table.Column<string>(maxLength: 1000, nullable: true),
                    notConfirmDescription = table.Column<string>(maxLength: 1000, nullable: true),
                    link = table.Column<string>(maxLength: 1000, nullable: true),
                    adminConfirmStatus = table.Column<int>(nullable: false),
                    userId = table.Column<int>(nullable: false),
                    code = table.Column<string>(nullable: true),
                    createDate = table.Column<DateTime>(maxLength: 50, nullable: false),
                    expireDate = table.Column<DateTime>(maxLength: 50, nullable: false),
                    expireDateIsespacial = table.Column<DateTime>(maxLength: 50, nullable: false),
                    categoryId = table.Column<int>(nullable: false),
                    countView = table.Column<int>(nullable: false),
                    cityId = table.Column<int>(nullable: false),
                    provinceId = table.Column<int>(nullable: false),
                    areaId = table.Column<int>(nullable: false),
                    isSpecial = table.Column<bool>(nullable: false),
                    isEmergency = table.Column<bool>(nullable: false),
                    ExpireDateEmergency = table.Column<DateTime>(nullable: true),
                    isPaid = table.Column<bool>(nullable: false),
                    deletedAt = table.Column<DateTime>(nullable: true),
                    AdminUserAccepted = table.Column<string>(nullable: true),
                    AcceptedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notices", x => x.id);
                    table.ForeignKey(
                        name: "FK_Notices_Areas_areaId",
                        column: x => x.areaId,
                        principalTable: "Areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notices_Categorys_categoryId",
                        column: x => x.categoryId,
                        principalTable: "Categorys",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notices_Cities_cityId",
                        column: x => x.cityId,
                        principalTable: "Cities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notices_Provinces_provinceId",
                        column: x => x.provinceId,
                        principalTable: "Provinces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notices_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersToDiscountCodes",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DiscountCodeId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersToDiscountCodes", x => x.id);
                    table.ForeignKey(
                        name: "FK_UsersToDiscountCodes_DiscountCodes_DiscountCodeId",
                        column: x => x.DiscountCodeId,
                        principalTable: "DiscountCodes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersToDiscountCodes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdminsInProvices",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    userId = table.Column<int>(nullable: true),
                    adminsInCityId = table.Column<int>(nullable: true),
                    provinceId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminsInProvices", x => x.id);
                    table.ForeignKey(
                        name: "FK_AdminsInProvices_AdminsInCities_adminsInCityId",
                        column: x => x.adminsInCityId,
                        principalTable: "AdminsInCities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdminsInProvices_Provinces_provinceId",
                        column: x => x.provinceId,
                        principalTable: "Provinces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdminsInProvices_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BannerImage",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    FileLocation = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    BannerId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BannerImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BannerImage_Banner_BannerId",
                        column: x => x.BannerId,
                        principalTable: "Banner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdminActivities",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    userId = table.Column<int>(nullable: true),
                    date = table.Column<DateTime>(nullable: false),
                    activityType = table.Column<int>(nullable: false),
                    noticeId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminActivities", x => x.id);
                    table.ForeignKey(
                        name: "FK_AdminActivities_Notices_noticeId",
                        column: x => x.noticeId,
                        principalTable: "Notices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdminActivities_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Factors",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    userId = table.Column<int>(nullable: false),
                    noticeId = table.Column<long>(nullable: true),
                    bannerId = table.Column<long>(nullable: true),
                    state = table.Column<int>(nullable: false),
                    factorKind = table.Column<int>(nullable: false),
                    createDatePersian = table.Column<string>(maxLength: 50, nullable: false),
                    totalPrice = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factors", x => x.id);
                    table.ForeignKey(
                        name: "FK_Factors_Banner_bannerId",
                        column: x => x.bannerId,
                        principalTable: "Banner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Factors_Notices_noticeId",
                        column: x => x.noticeId,
                        principalTable: "Notices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Factors_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    text = table.Column<string>(nullable: true),
                    date = table.Column<DateTime>(nullable: false),
                    rreceiverId = table.Column<int>(nullable: false),
                    ssenderId = table.Column<int>(nullable: false),
                    MessageType = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false),
                    Noticeid = table.Column<long>(nullable: true),
                    senderid = table.Column<int>(nullable: true),
                    receiverid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.id);
                    table.ForeignKey(
                        name: "FK_Messages_Notices_Noticeid",
                        column: x => x.Noticeid,
                        principalTable: "Notices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Users_receiverid",
                        column: x => x.receiverid,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Users_senderid",
                        column: x => x.senderid,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NoticeImages",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    image = table.Column<string>(maxLength: 500, nullable: true),
                    noticeId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoticeImages", x => x.id);
                    table.ForeignKey(
                        name: "FK_NoticeImages_Notices_noticeId",
                        column: x => x.noticeId,
                        principalTable: "Notices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportNotices",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    userId = table.Column<int>(nullable: false),
                    noticeId = table.Column<long>(nullable: false),
                    message = table.Column<string>(maxLength: 5000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportNotices", x => x.id);
                    table.ForeignKey(
                        name: "FK_ReportNotices_Notices_noticeId",
                        column: x => x.noticeId,
                        principalTable: "Notices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportNotices_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserFavorites",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    noticeId = table.Column<long>(nullable: false),
                    userId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavorites", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserFavorites_Notices_noticeId",
                        column: x => x.noticeId,
                        principalTable: "Notices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserFavorites_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VisitNotices",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    noticeId = table.Column<long>(nullable: false),
                    date = table.Column<DateTime>(maxLength: 50, nullable: false),
                    countView = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitNotices", x => x.id);
                    table.ForeignKey(
                        name: "FK_VisitNotices_Notices_noticeId",
                        column: x => x.noticeId,
                        principalTable: "Notices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitNoticeUsers",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    noticeId = table.Column<long>(nullable: false),
                    userId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitNoticeUsers", x => x.id);
                    table.ForeignKey(
                        name: "FK_VisitNoticeUsers_Notices_noticeId",
                        column: x => x.noticeId,
                        principalTable: "Notices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisitNoticeUsers_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FactorItems",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    price = table.Column<long>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    FactorId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactorItems", x => x.id);
                    table.ForeignKey(
                        name: "FK_FactorItems_Factors_FactorId",
                        column: x => x.FactorId,
                        principalTable: "Factors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FactorItems_Notices_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Notices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminActivities_noticeId",
                table: "AdminActivities",
                column: "noticeId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminActivities_userId",
                table: "AdminActivities",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminsInCities_cityId",
                table: "AdminsInCities",
                column: "cityId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminsInCities_userid",
                table: "AdminsInCities",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_AdminsInProvices_adminsInCityId",
                table: "AdminsInProvices",
                column: "adminsInCityId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminsInProvices_provinceId",
                table: "AdminsInProvices",
                column: "provinceId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminsInProvices_userId",
                table: "AdminsInProvices",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Areas_provinceId",
                table: "Areas",
                column: "provinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Banner_userId",
                table: "Banner",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_BannerImage_BannerId",
                table: "BannerImage",
                column: "BannerId");

            migrationBuilder.CreateIndex(
                name: "IX_Categorys_parentCategoryId",
                table: "Categorys",
                column: "parentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FactorItems_FactorId",
                table: "FactorItems",
                column: "FactorId");

            migrationBuilder.CreateIndex(
                name: "IX_FactorItems_ProductId",
                table: "FactorItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Factors_bannerId",
                table: "Factors",
                column: "bannerId");

            migrationBuilder.CreateIndex(
                name: "IX_Factors_noticeId",
                table: "Factors",
                column: "noticeId");

            migrationBuilder.CreateIndex(
                name: "IX_Factors_userId",
                table: "Factors",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_InformationMedias_InformationId",
                table: "InformationMedias",
                column: "InformationId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Noticeid",
                table: "Messages",
                column: "Noticeid");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_receiverid",
                table: "Messages",
                column: "receiverid");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_senderid",
                table: "Messages",
                column: "senderid");

            migrationBuilder.CreateIndex(
                name: "IX_NoticeImages_noticeId",
                table: "NoticeImages",
                column: "noticeId");

            migrationBuilder.CreateIndex(
                name: "IX_Notices_areaId",
                table: "Notices",
                column: "areaId");

            migrationBuilder.CreateIndex(
                name: "IX_Notices_categoryId",
                table: "Notices",
                column: "categoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Notices_cityId",
                table: "Notices",
                column: "cityId");

            migrationBuilder.CreateIndex(
                name: "IX_Notices_provinceId",
                table: "Notices",
                column: "provinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Notices_userId",
                table: "Notices",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Provinces_cityId",
                table: "Provinces",
                column: "cityId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportNotices_noticeId",
                table: "ReportNotices",
                column: "noticeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportNotices_userId",
                table: "ReportNotices",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavorites_noticeId",
                table: "UserFavorites",
                column: "noticeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavorites_userId",
                table: "UserFavorites",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_provinceId",
                table: "Users",
                column: "provinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_roleId",
                table: "Users",
                column: "roleId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersToDiscountCodes_DiscountCodeId",
                table: "UsersToDiscountCodes",
                column: "DiscountCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersToDiscountCodes_UserId",
                table: "UsersToDiscountCodes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitNotices_noticeId",
                table: "VisitNotices",
                column: "noticeId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitNoticeUsers_noticeId",
                table: "VisitNoticeUsers",
                column: "noticeId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitNoticeUsers_userId",
                table: "VisitNoticeUsers",
                column: "userId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AboutUss");

            migrationBuilder.DropTable(
                name: "AdminActivities");

            migrationBuilder.DropTable(
                name: "AdminsInProvices");

            migrationBuilder.DropTable(
                name: "BannerImage");

            migrationBuilder.DropTable(
                name: "ContactUss");

            migrationBuilder.DropTable(
                name: "FactorItems");

            migrationBuilder.DropTable(
                name: "InformationMedias");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "NoticeImages");

            migrationBuilder.DropTable(
                name: "PaymentRequestAttemps");

            migrationBuilder.DropTable(
                name: "ReportNotices");

            migrationBuilder.DropTable(
                name: "Rules");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Sliders");

            migrationBuilder.DropTable(
                name: "StaticPrices");

            migrationBuilder.DropTable(
                name: "Stirs");

            migrationBuilder.DropTable(
                name: "UserFavorites");

            migrationBuilder.DropTable(
                name: "UsersToDiscountCodes");

            migrationBuilder.DropTable(
                name: "VisitNotices");

            migrationBuilder.DropTable(
                name: "VisitNoticeUsers");

            migrationBuilder.DropTable(
                name: "AdminsInCities");

            migrationBuilder.DropTable(
                name: "Factors");

            migrationBuilder.DropTable(
                name: "Informations");

            migrationBuilder.DropTable(
                name: "DiscountCodes");

            migrationBuilder.DropTable(
                name: "Banner");

            migrationBuilder.DropTable(
                name: "Notices");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "Categorys");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Provinces");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Cities");
        }
    }
}
