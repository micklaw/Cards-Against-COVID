// OpenTelemetry setup for React application
import { WebTracerProvider } from '@opentelemetry/sdk-trace-web';
import { getWebAutoInstrumentations } from '@opentelemetry/auto-instrumentations-web';
import { registerInstrumentations } from '@opentelemetry/instrumentation';
import { Resource } from '@opentelemetry/resources';
import { ATTR_SERVICE_NAME } from '@opentelemetry/semantic-conventions';
import { BatchSpanProcessor } from '@opentelemetry/sdk-trace-base';
import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-http';

export function initializeTelemetry() {
  const provider = new WebTracerProvider({
    resource: new Resource({
      [ATTR_SERVICE_NAME]: 'CardsAgainstHumanity.Web',
    }),
  });

  // Configure exporter based on environment
  const instrumentationKey = import.meta.env.VITE_APPINSIGHTS_KEY;
  
  if (instrumentationKey) {
    const exporter = new OTLPTraceExporter({
      url: `https://dc.services.visualstudio.com/v2/track`,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    provider.addSpanProcessor(new BatchSpanProcessor(exporter));
  } else {
    console.warn('Application Insights key not configured');
  }

  provider.register();

  // Auto-instrument browser APIs
  registerInstrumentations({
    instrumentations: [
      getWebAutoInstrumentations({
        '@opentelemetry/instrumentation-fetch': {
          propagateTraceHeaderCorsUrls: /.*/,
          clearTimingResources: true,
        },
        '@opentelemetry/instrumentation-xml-http-request': {
          propagateTraceHeaderCorsUrls: /.*/,
        },
      }),
    ],
  });

  return provider;
}
