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

        return $.ajax("/api/job", {
                method: "PUT",
                data: { Foo: 4 }
            })
            .then(id => {

                var deferred = $.Deferred<any>();

                try {
                    var eventSource = new EventSource(`/api/job/${id}/progress`);
                    eventSource.onmessage = message => {
                        deferred.notify(JSON.parse(message.data));
                    };

                    // eventSource.onerror = error => alert(JSON.stringify(error));
                }
                catch (error) {
                }
                
                $.getJSON(`/api/job/${id}`)
                    .then(result => deferred.resolve(result))
                    .fail(error => deferred.reject(error));

                return deferred.promise();
            });
    }
}