export default function Unauthorized() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100">
      <div className="bg-white p-6 rounded shadow text-center">
        <h1 className="text-2xl font-bold text-red-600">Oops!</h1>
        <p className="mt-2">You can’t go to that page.</p>
        <a
          href="/login"
          className="mt-4 inline-block bg-blue-600 text-white px-4 py-2 rounded"
        >
          Go to Login
        </a>
      </div>
    </div>
  );
}
