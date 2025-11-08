export default function Features() {
  const features = [
    { title: "Expert Trainers", desc: "Certified professionals guiding your fitness journey." },
    { title: "Modern Equipment", desc: "State-of-the-art machines and free weights." },
    { title: "Personal Training", desc: "Customized workout plans for your goals." },
  ];

  return (
    <section className="py-20 bg-white">
      <div className="container mx-auto px-6 md:px-20 text-center">
        <h2 className="text-4xl font-bold mb-12">Our Features</h2>
        <div className="grid md:grid-cols-3 gap-10">
          {features.map((f, i) => (
            <div key={i} className="p-6 bg-gray-50 rounded-xl shadow hover:shadow-lg transition">
              <h3 className="text-xl font-semibold mb-2">{f.title}</h3>
              <p className="text-gray-600">{f.desc}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
