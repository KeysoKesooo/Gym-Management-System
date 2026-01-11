"use client";

import { BaseModal } from "@/components/modals/baseModal";
import { Form, FormItem, FormLabel, FormControl, FormMessage } from "@/components/ui/form";
import { useForm, Controller } from "react-hook-form";
import { Input } from "@/components/ui/input"; // or your preferred input component

interface ModalFormProps<T> {
  open: boolean;
  onClose: () => void;
  onSubmit: (data: T) => void;
  title: string;
  description?: string;
  defaultValues: T;
  confirmLabel?: string;
}

export function ModalForm<T extends Record<string, any>>({
  open,
  onClose,
  onSubmit,
  title,
  description,
  defaultValues,
  confirmLabel = "Submit",
}: ModalFormProps<T>) {
  const form = useForm<T>({ defaultValues });

  const handleSubmit = form.handleSubmit((values) => {
    onSubmit(values);
    onClose();
    form.reset();
  });

  return (
    <BaseModal open={open} onClose={onClose} title={title} description={description} onConfirm={handleSubmit} confirmLabel={confirmLabel}>
      <Form {...form}>
        <form className="flex flex-col gap-4">
          {Object.keys(defaultValues).map((key) => (
            <FormItem key={key}>
              <FormLabel>{key.charAt(0).toUpperCase() + key.slice(1)}</FormLabel>
              <FormControl>
                <Controller
                  control={form.control}
                  name={key as any}
                  render={({ field }) => (
                    <Input
                      {...field}
                      type={key === "password" ? "password" : "text"}
                      placeholder={key}
                    />
                  )}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          ))}
        </form>
      </Form>
    </BaseModal>
  );
}
