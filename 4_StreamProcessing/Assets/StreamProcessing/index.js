const storage = require('azure-storage');
const tableName = 'TaxiSensorData';

module.exports = function (context, eventHubMessages) {
    const connectionString = process.env['CosmosDBConnectionString'];
    const storageClient = storage.createTableService(connectionString);
    let index = 1;

    storageClient.createTableIfNotExists(tableName, function (error, result, response) {
        context.log("eventHubMessages length: " + eventHubMessages.length);

        if (!error) {
            eventHubMessages.forEach(item => {
                let obj = {
                    PartitionKey: { '_': "taxi_" + item.name },
                    RowKey: { '_': Date.now().toString() + "_" + (index++) }
                };

                for (var attrname in item) {
                    if (attrname === 'name') continue;
                    obj[attrname] = { '_': item[attrname] };
                }

                storageClient.insertEntity(tableName, obj, function (error, result, response) {
                    if (!error) {
                        context.log('Insert entity');
                    }
                    else {
                        context.log(error);
                    }
                });
            });
        }
        else {
            context.log(error);
        }
    });


    context.done();
};