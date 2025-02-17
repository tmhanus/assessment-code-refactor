### Production Readiness Report ###

- Map custom exceptions into property HTTP response codes - in middleware
- Design proper REST API Requests with JSON payloads for POSTs  
```
POST /api/products/{product-id}/processOrder
{
  "ProductType": "Type1",
  "PaymentType": "PayPal"
}
```

- Validation
  - Validate requests - using fluent validation and pipeline behavior of mediatr for example

- Security
  - Enable Authentication & Enforce Authorization
  - Don't store secrets/connection strings in codebase - maybe KeyVault instead

- Compliance
  - Implement log redaction

- HTTP Client
  - Retry Policy (i.e. retry n-times on recoverable errors)
  - Circuit breaker policy

- Database
  - more production ready DB should be used (i.e. PostgreSql or MSSQL)
  - Use proper configuration for EF Core entities - ideally using ConfigurationFiles
  - if app grows more - Proper Repository pattern could be implemented
  - if app use cases grow more complex - Unit of Work pattern could be introduced
  - handle decimals precision
  - introduce migrations

- Emails
  - sending emails could be handled asynchronously - background email sender

- Observability
  - Integrate OpenTelemetry for monitoring
  - Add Healthcheck
  - Configure proper Serilog sinks - i.e. log into AppInsights, or ELK
  - Add proper logs where necessary - i.e. Info/Debug/Verbose logs

- Containerize the solution

- Tests
  - add Integration tests against real DB - using WebApplicationFactory

- Delivery
  - have automated CI/CD pipeline

- Dev Experience
  - introduce .editorconfig