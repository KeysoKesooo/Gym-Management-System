// components/ui/data-table.tsx
"use client";

import { useState, useMemo } from "react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

export interface Column<T> {
  label: string;
  accessor: keyof T;
  render?: (row: T) => React.ReactNode;
}

interface DataTableProps<T> {
  columns: Column<T>[];
  data: T[];
  pageSize?: number;
}

export function DataTable<T>({
  columns,
  data,
  pageSize = 5,
}: DataTableProps<T>) {
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);

  const filtered = useMemo(
    () =>
      data.filter((row) =>
        Object.values(row as any).some((value) =>
          String(value).toLowerCase().includes(search.toLowerCase())
        )
      ),
    [search, data]
  );

  const paginated = filtered.slice((page - 1) * pageSize, page * pageSize);

  return (
    <div>
      <Input
        placeholder="Search..."
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        className="mb-4"
      />

      <table className="w-full border-collapse">
        <thead className="bg-muted">
          <tr>
            {columns.map((col) => (
              <th key={String(col.accessor)} className="p-2 border text-left">
                {col.label}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {paginated.map((row, i) => (
            <tr key={i} className="border-b hover:bg-muted/50">
              {columns.map((col) => (
                <td key={String(col.accessor)} className="p-2">
                  {col.render
                    ? col.render(row)
                    : String(row[col.accessor])}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>

      <div className="flex justify-between mt-4">
        <Button
          variant="outline"
          disabled={page === 1}
          onClick={() => setPage((p) => p - 1)}
        >
          Prev
        </Button>
        <Button
          variant="outline"
          disabled={page * pageSize >= filtered.length}
          onClick={() => setPage((p) => p + 1)}
        >
          Next
        </Button>
      </div>
    </div>
  );
}
