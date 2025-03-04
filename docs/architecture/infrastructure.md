# Infrastructure Architecture

This document outlines the infrastructure architecture for the OuiAI platform, covering cloud resources, deployment processes, and operational considerations.

## Overview

OuiAI is deployed on Microsoft Azure, utilizing a containerized approach with Kubernetes for orchestration. The architecture is designed to be scalable, resilient, and secure.

## Cloud Resources

### Azure Services Used

| Service | Purpose |
|---------|---------|
| Azure Kubernetes Service (AKS) | Container orchestration for microservices |
| Azure SQL Database | Relational databases for microservices |
| Azure Redis Cache | Distributed caching |
| Azure Service Bus | Message broker for async communication |
| Azure Storage | Blob storage for files and static content |
| Azure CDN | Content delivery network |
| Azure Key Vault | Secrets management |
| Azure Active Directory | Identity management (for admin users) |
| Azure Application Insights | Application monitoring |
| Azure Log Analytics | Centralized logging |
| Azure Container Registry | Docker image repository |
| Azure DevOps | CI/CD pipelines |

## Infrastructure Diagram

![OuiAI Infrastructure Diagram](./images/ouiai-infrastructure.png)

## Kubernetes Architecture

OuiAI's microservices are deployed to Azure Kubernetes Service with the following components:

### Namespaces

- **ouiai-prod**: Production environment
- **ouiai-staging**: Staging environment
- **ouiai-dev**: Development environment
- **monitoring**: Monitoring tools (Prometheus, Grafana)
- **ingress-nginx**: Ingress controller

### Workloads

#### Deployments

Each microservice has its own deployment configuration:

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: identity-service
  namespace: ouiai-prod
spec:
  replicas: 3
  selector:
    matchLabels:
      app: identity-service
  template:
    metadata:
      labels:
        app: identity-service
    spec:
      containers:
      - name: identity-service
        image: ouiai.azurecr.io/identity-service:latest
        ports:
        - containerPort: 80
        resources:
          requests:
            cpu: 100m
            memory: 128Mi
          limits:
            cpu: 500m
            memory: 512Mi
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: Production
        # Additional environment variables
        livenessProbe:
          httpGet:
            path: /health
            port: 80
          initialDelaySeconds: 30
          timeoutSeconds: 5
        readinessProbe:
          httpGet:
            path: /ready
            port: 80
          initialDelaySeconds: 15
          timeoutSeconds: 5
```

#### StatefulSets

Used for services that require stable network identifiers or persistent storage:

- Redis for caching
- Elasticsearch for search

#### DaemonSets

Used for:
- Log collectors (Fluentd)
- Monitoring agents

### Services

Each microservice has a corresponding Kubernetes service:

```yaml
apiVersion: v1
kind: Service
metadata:
  name: identity-service
  namespace: ouiai-prod
spec:
  selector:
    app: identity-service
  ports:
  - port: 80
    targetPort: 80
  type: ClusterIP
```

### Ingress

API Gateway exposed through Nginx Ingress Controller:

```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: ouiai-ingress
  namespace: ouiai-prod
  annotations:
    kubernetes.io/ingress.class: nginx
    nginx.ingress.kubernetes.io/ssl-redirect: "true"
    cert-manager.io/cluster-issuer: letsencrypt-prod
spec:
  tls:
  - hosts:
    - api.ouiai.com
    secretName: ouiai-tls
  rules:
  - host: api.ouiai.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: gateway-service
            port:
              number: 80
```

### Storage

Persistent Volume Claims for stateful services:

```yaml
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: elasticsearch-data
  namespace: ouiai-prod
spec:
  accessModes:
    - ReadWriteOnce
  storageClassName: managed-premium
  resources:
    requests:
      storage: 100Gi
```

### ConfigMaps and Secrets

ConfigMaps for service configuration:
- Application settings
- Feature flags
- Environment-specific configurations

Secrets for sensitive information:
- Database connection strings
- API keys
- JWT signing keys

## Azure SQL Database

Each microservice has its own dedicated database:

| Database | Purpose |
|----------|---------|
| OuiAI_Identity | User accounts and profiles |
| OuiAI_Projects | Project data and metadata |
| OuiAI_Social | Social interactions and activity feeds |
| OuiAI_Notifications | Notification templates and delivery status |
| OuiAI_Search | Search indexes and analytics |

## Network Security

### Network Topology

- **Public Ingress**: Only the API Gateway is exposed publicly
- **Private Network**: Internal service-to-service communication
- **Network Policies**: Restrict pod-to-pod communication

### Azure Network Security

- **Virtual Network**: AKS cluster in dedicated VNet
- **Network Security Groups**: Control traffic flow
- **Private Endpoints**: For Azure SQL and other services
- **DDoS Protection**: Azure DDoS Protection Standard

## Monitoring and Observability

### Application Monitoring

- **Application Insights**: APM for all services
- **Distributed Tracing**: Trace requests across services
- **Exception Tracking**: Capture and analyze exceptions
- **Performance Metrics**: Response times, throughput, etc.

### Infrastructure Monitoring

- **Azure Monitor**: VM and container monitoring
- **Prometheus**: Custom metrics collection
- **Grafana**: Visualization and dashboards
- **Alerting**: Email and webhook notifications

### Logging

- **Centralized Logging**: All logs sent to Log Analytics
- **Structured Logging**: JSON-formatted logs
- **Log Levels**: Information, Warning, Error, Debug
- **Context Enrichment**: Request IDs, user IDs, etc.

## Backup and Disaster Recovery

### Database Backups

- **Automated Backups**: Daily full backups
- **Transaction Log Backups**: Hourly
- **Geo-Replication**: Secondary region for disaster recovery
- **Point-in-Time Restore**: Up to 35 days

### Application Recovery

- **Multi-Region Deployment**: Active-passive configuration
- **Recovery Time Objective (RTO)**: 1 hour
- **Recovery Point Objective (RPO)**: 5 minutes
- **Disaster Recovery Testing**: Quarterly test exercises

## CI/CD Pipeline

### Build Pipeline

1. **Source Control**: GitHub repository
2. **Build Trigger**: Pull request or commit to main
3. **Build Agent**: Azure DevOps hosted agent
4. **Build Steps**:
   - Restore dependencies
   - Compile code
   - Run unit tests
   - Run static code analysis
   - Build Docker images
   - Push to Azure Container Registry

### Release Pipeline

1. **Environments**: Dev, Staging, Production
2. **Deployment Strategy**: Blue-green deployment
3. **Approval Gates**: Manual approval for production
4. **Deployment Steps**:
   - Update Kubernetes deployments
   - Run database migrations
   - Run integration tests
   - Health check verification

## Infrastructure as Code

OuiAI infrastructure is defined as code using:

1. **Terraform**: For Azure resources
2. **Kubernetes YAML**: For Kubernetes resources
3. **Azure DevOps Pipelines**: For CI/CD definitions
4. **ARM Templates**: For Azure-specific configurations

Example Terraform configuration:

```hcl
resource "azurerm_kubernetes_cluster" "aks" {
  name                = "ouiai-aks-cluster"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  dns_prefix          = "ouiai"

  default_node_pool {
    name       = "default"
    node_count = 3
    vm_size    = "Standard_DS2_v2"
  }

  identity {
    type = "SystemAssigned"
  }

  addon_profile {
    http_application_routing {
      enabled = true
    }
  }
}
```

## Scaling Strategy

### Horizontal Scaling

- **Microservices**: Kubernetes Horizontal Pod Autoscaler (HPA)
- **Databases**: Scale read replicas or storage
- **Cache**: Redis cluster scaling

### Vertical Scaling

- **Kubernetes Nodes**: VM size upgrades
- **Databases**: Instance size upgrades

### Auto-Scaling Rules

- **CPU Utilization**: Scale when CPU > 70%
- **Memory Utilization**: Scale when Memory > 80%
- **Request Rate**: Scale based on incoming request rate
- **Queue Length**: Scale based on message queue length

## Security Practices

### Application Security

- **Authentication**: JWT-based authentication
- **Authorization**: Role-based access control
- **Input Validation**: Server-side validation
- **Output Encoding**: Prevent XSS
- **HTTPS**: TLS 1.3 for all traffic

### Infrastructure Security

- **Container Security**: Regular scanning for vulnerabilities
- **Network Security**: Network policies and isolation
- **Secret Management**: Azure Key Vault integration
- **RBAC**: Role-based access control for Azure resources
- **WAF**: Web Application Firewall for public endpoints

## Compliance and Governance

- **Logging and Auditing**: All administrative actions logged
- **Resource Tagging**: Consistent tagging strategy
- **Cost Management**: Resource budgets and alerts
- **Policy Enforcement**: Azure Policy for compliance
- **Regular Reviews**: Security and architecture reviews
