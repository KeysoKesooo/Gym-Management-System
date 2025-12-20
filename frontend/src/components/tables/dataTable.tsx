"use client";

import { useState, useMemo } from "react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

interface Column {
  label: string;
  accessor: string;
}

interface DataTableProps {
  columns: Column[];
  data: any[];
  pageSize?: number;
}

export function DataTable({ columns, data, pageSize = 5 }: DataTableProps) {
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);

  const filtered = useMemo(
    () =>
      data.filter((row) =>
        Object.values(row).some((value) =>
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
        <thead className="bg-gray-100">
          <tr>
            {columns.map((col) => (
              <th key={col.accessor} className="p-2 border text-left">
                {col.label}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {paginated.map((row, i) => (
            <tr key={i} className="hover:bg-gray-50 border-b">
              {columns.map((col) => (
                <td key={col.accessor} className="p-2">
                  {row[col.accessor]}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>

      <div className="flex justify-between mt-4">
        <Button disabled={page === 1} onClick={() => setPage((p) => p - 1)} variant="outline">
          Prev
        </Button>
        <Button disabled={page * pageSize >= filtered.length} onClick={() => setPage((p) => p + 1)} variant="outline">
          Next
        </Button>
      </div>
    </div>
  );
}
