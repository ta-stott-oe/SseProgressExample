///<reference path="typings/jquery/jquery.d.ts" />

interface Callback { (data: any): void; }

declare class EventSourceMessage {
    data: string;
}

declare class EventSource {
    onmessage: {
        (message: EventSourceMessage): void
    };
    addEventListener(event: string, cb: Callback): void;
    constructor(name: string);
}

module Job {

    export function DoJob(): JQueryPromise<any> {

        // Call PUT method to start job 
        return $.ajax("/api/job", {
                method: "PUT",
                data: { Foo: 4 } // Job parameters
            })
            // Response is stringy id for job
            .then(id => {

                // Create deferred object explicitly so we can send progress updates
                var deferred = $.Deferred<any>();

                // Point EventSource at progress method for this job
                try {
                    var eventSource = new EventSource(`/api/job/${id}/progress`);
                    eventSource.onmessage = message => {
                        deferred.notify(JSON.parse(message.data)); // Messages have "data" property which in our case is JSON
                    };

                    // Could perhaps handle progress reporting errors here
                }
                catch (error) {

                    // Catch any errors thrown while creating EventSource, we can carry on without it
                }

                // Call GET method to get job result and wire it up to deferred object
                $.getJSON(`/api/job/${id}`)
                    .then(result => deferred.resolve(result))
                    .fail(error => deferred.reject(error));

                return deferred.promise();
            });
    }
}