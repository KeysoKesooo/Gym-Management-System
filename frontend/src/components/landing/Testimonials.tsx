export default function Testimonials() {
  const testimonials = [
    { name: "John Doe", feedback: "The trainers are amazing! I finally reached my goals." },
    { name: "Jane Smith", feedback: "Love the environment and equipment. Highly recommend!" },
  ];

  return (
    <section className="py-20 bg-gray-100">
      <div className="container mx-auto px-6 md:px-20 text-center">
        <h2 className="text-4xl font-bold mb-12">Testimonials</h2>
        <div className="grid md:grid-cols-2 gap-10">
          {testimonials.map((t, i) => (
            <div key={i} className="p-6 bg-white rounded-xl shadow">
              <p className="text-gray-600 mb-4">"{t.feedback}"</p>
              <h3 className="font-semibold">{t.name}</h3>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
