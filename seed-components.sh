
#!/bin/bash

# Replace this with the URL your API is running on.
API_URL="${API_URL:-http://localhost:5099/api/components}"

components='[
  {"name":"Silver Round Bead 6mm","color":"Silver","category":"Beads","unitOfMeasurement":"pcs","unitCost":1.25,"lowStockThreshold":20,"storageLocation":"A1","barcode":"7350000000011","initialQuantity":0},
  {"name":"Gold Round Bead 6mm","color":"Gold","category":"Beads","unitOfMeasurement":"pcs","unitCost":1.75,"lowStockThreshold":20,"storageLocation":"A2","barcode":"7350000000028","initialQuantity":0},
  {"name":"Rose Quartz Bead 8mm","color":"Pink","category":"Gemstones","unitOfMeasurement":"pcs","unitCost":3.50,"lowStockThreshold":15,"storageLocation":"B1","barcode":"7350000000035","initialQuantity":0},
  {"name":"Amethyst Bead 8mm","color":"Purple","category":"Gemstones","unitOfMeasurement":"pcs","unitCost":4.20,"lowStockThreshold":15,"storageLocation":"B2","barcode":"7350000000042","initialQuantity":0},
  {"name":"Elastic Jewelry Cord","color":"Transparent","category":"Cord","unitOfMeasurement":"meters","unitCost":2.50,"lowStockThreshold":10,"storageLocation":"C1","barcode":"7350000000059","initialQuantity":0},
  {"name":"Sterling Silver Lobster Clasp","color":"Silver","category":"Clasps","unitOfMeasurement":"pcs","unitCost":8.90,"lowStockThreshold":10,"storageLocation":"D1","barcode":"7350000000066","initialQuantity":0},
  {"name":"Gold Plated Lobster Clasp","color":"Gold","category":"Clasps","unitOfMeasurement":"pcs","unitCost":6.50,"lowStockThreshold":10,"storageLocation":"D2","barcode":"7350000000073","initialQuantity":0},
  {"name":"Stainless Steel Jump Ring 5mm","color":"Silver","category":"Findings","unitOfMeasurement":"pcs","unitCost":0.35,"lowStockThreshold":50,"storageLocation":"E1","barcode":"7350000000080","initialQuantity":0},
  {"name":"Freshwater Pearl 7mm","color":"White","category":"Pearls","unitOfMeasurement":"pcs","unitCost":5.75,"lowStockThreshold":15,"storageLocation":"B3","barcode":"7350000000097","initialQuantity":0},
  {"name":"Silver Heart Charm","color":"Silver","category":"Charms","unitOfMeasurement":"pcs","unitCost":12.50,"lowStockThreshold":10,"storageLocation":"F1","barcode":"7350000000103","initialQuantity":0}
]'

echo "$components" | jq -c '.[]' | while IFS= read -r component; do
    curl --fail-with-body -sS \
        -X POST "$API_URL" \
        -H "Content-Type: application/json" \
        -d "$component"
    echo
done
