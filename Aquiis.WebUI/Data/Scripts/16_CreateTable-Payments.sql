CREATE TABLE IF NOT EXISTS "Payments" (
    "Id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "LeaseId" INTEGER NOT NULL,
    "UserId" TEXT NOT NULL,
    "InvoiceId" INTEGER NOT NULL,
    "Description" TEXT,
    "PaymentDate" TEXT,
    "Amount" REAL,
    "PaymentMethod" TEXT, --Cash, Check, CreditCard, BankTransfer
    "ReferenceNumber" TEXT,
    "PaymentStatus" TEXT NOT NULL DEFAULT 'Completed', --Completed, Pending, Failed
    "Notest" TEXT,
    "Status" TEXT NOT NULL DEFAULT 'Pending', --Pending, Paid, Overdue, Cancelled
    "Notes" TEXT,
    "CreatedDate" TEXT NOT NULL DEFAULT (datetime('now')),
    "CreatedBy" TEXT NOT NULL DEFAULT '',
    "LastModified" TEXT,
    "LastModifiedBy" TEXT NOT NULL DEFAULT '',
    "IsDeleted" INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY("LeaseId") REFERENCES "Leases"("Id")
);