# OuiAI.Common

Ce projet contient des composants partagés utilisés par les différents microservices OuiAI.

## Structure du projet

### Extensions

Le dossier `Extensions` contient des classes d'extension pour configurer les services et les applications de manière cohérente à travers les microservices :

- **ServiceCollectionExtensions.cs** : Extensions pour configurer les services communs (JWT, CORS, Swagger, etc.)
- **ApplicationBuilderExtensions.cs** : Extensions pour configurer le pipeline HTTP commun
- **DatabaseExtensions.cs** : Extensions pour la configuration et la migration des bases de données
- **SignalRExtensions.cs** : Extensions pour la configuration de SignalR

### Interfaces

Le dossier `Interfaces` contient les contrats pour les services communs :

- **IServiceBusPublisher.cs** : Interface pour le service de publication de messages sur le bus de service

### Services

Le dossier `Services` contient les implémentations des services communs :

- **ServiceBusPublisher.cs** : Implémentation du service de publication de messages sur le bus de service

## Comment utiliser

### Configuration du Program.cs

Voici un exemple typique d'utilisation des extensions dans un fichier Program.cs :

```csharp
using OuiAI.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Common Services from Extensions
builder.Services.AddCommonJwtAuthentication(builder.Configuration);
builder.Services.AddCommonCorsPolicy();
builder.Services.AddSwaggerWithJwt("OuiAI API Title");
builder.Services.AddServiceBusPublisher(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline with common middleware
app.UseCommonMiddleware(app.Environment);

app.MapControllers();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.MigrateAndSeedDatabaseAsync<MyDbContext, Program>();
}

app.Run();
```

### Configuration de la base de données avec seeding

Pour initialiser une base de données avec des données de départ :

```csharp
await scope.ServiceProvider.MigrateAndSeedDatabaseAsync<MyDbContext, Program>(async context => 
{
    // Code de seeding
    if (!context.MyEntities.Any())
    {
        context.MyEntities.Add(new MyEntity { ... });
        await context.SaveChangesAsync();
    }
});
```

## Avantages

- **Réduction de la duplication de code** : Élimine le code dupliqué dans les fichiers Program.cs des microservices
- **Cohérence** : Garantit une configuration cohérente à travers tous les microservices
- **Maintenabilité** : Les modifications de configuration sont centralisées dans un seul endroit
- **Extensibilité** : Facilite l'ajout de nouvelles fonctionnalités communes à tous les microservices
